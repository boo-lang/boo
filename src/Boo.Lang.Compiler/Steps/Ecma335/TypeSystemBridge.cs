using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Reflection;

namespace Boo.Lang.Compiler.Steps.Ecma335
{
    internal class TypeSystemBridge
    {
        private readonly Dictionary<IType, EntityHandle> _typeLookup = new();
        private int _types = 0;
        private readonly Dictionary<IMethodBase, EntityHandle> _methodLookup = new();
        private readonly Dictionary<EntityHandle, IMethod> _reversemethodLookup = new();
        private readonly Dictionary<Assembly, EntityHandle> _assemblyLookup = new();
        private readonly Dictionary<IField, EntityHandle> _fieldLookup = new();
        private readonly Dictionary<IEvent, EntityHandle> _eventLookup = new();
        private readonly Dictionary<IProperty, EntityHandle> _propLookup = new();
        private readonly Dictionary<string, DocumentHandle> _documentLookup = new();
        private readonly TypeSystemServices _tss;
        private readonly List<(EntityHandle parent, GenericTypeParameterBuilder builder)> _genParamBuilders = new();
        private readonly List<(EntityHandle parent, TypeBuilder builder)> _typeBuilders = new();

        private const string BOO_LANG_GUID = "9d4ac6e1-65ce-4f04-8419-b743b0eb5718";
        private readonly GuidHandle _booLangGuidHandle;

        private int _methods = 0;
        private int _fields = 0;
        private int _events = 0;
        private int _props = 0;
        private int _params = 0;

        public TypeSystemBridge(MetadataBuilder builder, TypeSystemServices tss, Func<IType, Expression, object> getValue)
        {
            _tss = tss;
            AssemblyBuilder = builder;
            DebugBuilder = new();
            VoidTypeEntity = tss.VoidType;
            TypeTypeEntity = tss.TypeType;
            EnumTypeEntity = tss.EnumType;
            ValueTypeEntity = tss.ValueTypeType;
            ObjectTypeEntity = tss.ObjectType;

            /*
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var runtimeName = loadedAssemblies.Single(a => a.GetName().Name == "System.Runtime").GetName();
            var privateAsm = loadedAssemblies.Single(a => a.GetName().Name == "System.Private.CoreLib");
            var handle = HandleForAssemblyName(runtimeName);
            _assemblyLookup[privateAsm] = handle;
            */

            ObjectType = (TypeReferenceHandle)LookupType(tss.ObjectType);
            _getValue = getValue;
            _booLangGuidHandle = DebugBuilder.GetOrAddGuid(new Guid(BOO_LANG_GUID));
            NullImportHandle = DebugBuilder.AddImportScope(default, default);
        }

        public TypeReferenceHandle ObjectType { get; }

        private readonly Func<IType, Expression, object> _getValue;

        public IType VoidTypeEntity { get; }
        public IType TypeTypeEntity { get; }
        public IType EnumTypeEntity { get; }
        public IType ValueTypeEntity { get; }
        public IType ObjectTypeEntity { get; }

        public MetadataBuilder AssemblyBuilder { get; }
        public MetadataBuilder DebugBuilder { get; }
        public ImportScopeHandle NullImportHandle { get; }

        public EntityHandle TypeHandle(IType type) => _typeLookup[type];

        public EntityHandle ReserveType()
        {
            ++_types;
            EntityHandle result = MetadataTokens.TypeDefinitionHandle(_types);
            return result;
        }

        public EntityHandle ReserveType(IType type)
        {
            EntityHandle result = ReserveType();
            _typeLookup.Add(type, result);
            return result;
        }

        public EntityHandle LookupType(IType type)
        {
            if (_typeLookup.TryGetValue(type, out var result))
            {
                return result;
            }
            if (type.IsArray || type is AbstractGenericParameter || type.ConstructedInfo != null)
            {
                return LookupTypeSpec(type);
            }
            if (type is AnonymousCallableType act)
            {
                return LookupType(act.ConcreteType);
            }
            if (type is ExternalType et)
            {
                return LookupExternalType(et);
            }
            throw new EcmaBuildException($"Unable to lookup handle for type {type.QualifiedName()} of entity type {type.GetType()}.");
        }

        private readonly HashSet<IType> _eventMappedTypes = new();

        internal void EnsureEventMapForType(IType type, EventDefinitionHandle handle)
        {
            if (!_eventMappedTypes.Contains(type))
            {
                _eventMappedTypes.Add(type);
                AssemblyBuilder.AddEventMap((TypeDefinitionHandle)LookupType(type), handle);
            }
        }

        private readonly HashSet<IType> _propertyMappedTypes = new();

        internal void EnsurePropertyMapForType(IType type, PropertyDefinitionHandle handle)
        {
            if (!_propertyMappedTypes.Contains(type))
            {
                _propertyMappedTypes.Add(type);
                AssemblyBuilder.AddPropertyMap((TypeDefinitionHandle)LookupType(type), handle);
            }
        }

        private EntityHandle LookupExternalType(ExternalType type)
        {
            var name = type.ActualType.Name;
            var result = AssemblyBuilder.AddTypeReference(
                GetTypeScope(type),
                type.DeclaringType == null && type.ActualType.Namespace != null
                    ? AssemblyBuilder.GetOrAddString(type.ActualType.Namespace) 
                    : default,
                AssemblyBuilder.GetOrAddString(name));
            _typeLookup.Add(type, result);
            return result;
        }

        private EntityHandle GetTypeScope(ExternalType type)
        {
            if (type.DeclaringEntity is IType parentType)
            {
                return LookupType(parentType);
            }
            var parentAsm = type.ActualType.Assembly;
            if (_assemblyLookup.TryGetValue(parentAsm, out var result))
            {
                return result;
            }
            var asmName = parentAsm.GetName();
            var handle = HandleForAssemblyName(asmName);
            _assemblyLookup[parentAsm] = handle;
            return handle;
        }

        private EntityHandle HandleForAssemblyName(AssemblyName asmName)
        {
            var keyOrToken = asmName.GetPublicKeyToken() ?? asmName.GetPublicKey();
            return AssemblyBuilder.AddAssemblyReference(
                AssemblyBuilder.GetOrAddString(asmName.Name),
                asmName.Version,
                AssemblyBuilder.GetOrAddString(asmName.CultureName),
                AssemblyBuilder.GetOrAddBlob(keyOrToken),
                ConvertAssemblyNameFlags(asmName.Flags),
                default);
        }

        public static AssemblyFlags ConvertAssemblyNameFlags(AssemblyNameFlags flags)
        {
            var result = default(AssemblyFlags);
            if (flags.HasFlag(AssemblyNameFlags.Retargetable))
            {
                result |= AssemblyFlags.Retargetable;
            }
            if (flags.HasFlag(AssemblyNameFlags.EnableJITcompileOptimizer))
            {
                result |= AssemblyFlags.DisableJitCompileOptimizer;
            }
            if (flags.HasFlag(AssemblyNameFlags.EnableJITcompileTracking))
            {
                result |= AssemblyFlags.EnableJitCompileTracking;
            }
            return result;
        }

        private EntityHandle LookupTypeSpec(IType type)
        {
            var encoder = new BlobEncoder(new BlobBuilder()).TypeSpecificationSignature();
            EncodeType(encoder, type);

            var result = AssemblyBuilder.AddTypeSpecification(AssemblyBuilder.GetOrAddBlob(encoder.Builder));
            _typeLookup.Add(type, result);
            return result;
        }

        internal void EncodeTypeModified(SignatureTypeEncoder enc, IType type, IType modifier, bool required)
        {
            enc.CustomModifiers().AddModifier(LookupType(modifier), required);
            EncodeType(enc, type);
        }

        public void EncodeType(SignatureTypeEncoder enc, IType type)
        {
            if (type is IGenericParameter gp)
            {
                if (gp.DeclaringEntity.EntityType == EntityType.Type)
                {
                    enc.GenericTypeParameter(gp.GenericParameterPosition);
                }
                else
                {
                    enc.GenericMethodTypeParameter(gp.GenericParameterPosition);
                }
            }
            else if (type.ConstructedInfo != null)
            {
                EncodeConstructedType(enc, type.ConstructedInfo);
            }
            else if (type.GenericInfo != null)
            {
                EncodeUnconstructedType(enc, type);
            }
            else if (type.IsArray)
            {
                var arr = (IArrayType)type;
                if (arr.Rank == 1)
                {
                    EncodeType(enc.SZArray(), type.ElementType);
                }
                else
                {
                    enc.Array(out var elemEnc, out var shapeEnc);
                    EncodeType(elemEnc, type.ElementType);
                    shapeEnc.Shape(arr.Rank, ImmutableArray.Create<int>(), default);
                }
            }
            else if (type.IsPointer)
            {
                EncodeType(enc.Pointer(), type.ElementType);
            }
            else EncodeBasicType(enc, type);
        }

        private void EncodeUnconstructedType(SignatureTypeEncoder enc, IType type)
        {
            var gi = type.GenericInfo;
            var argEncoder = enc.GenericInstantiation(
                LookupType(type),
                gi.GenericParameters.Length,
                type.IsValueType);
            foreach (var param in gi.GenericParameters)
            {
                argEncoder.AddArgument().GenericTypeParameter(param.GenericParameterPosition);
            }
        }

        private void EncodeConstructedType(SignatureTypeEncoder enc, IConstructedTypeInfo ci)
        {
            var argEncoder = enc.GenericInstantiation(
                LookupType(ci.GenericDefinition),
                ci.GenericArguments.Length,
                ci.GenericDefinition.IsValueType);
            foreach (var arg in ci.GenericArguments)
            {
                EncodeType(argEncoder.AddArgument(), arg);
            }
        }

        private static readonly Dictionary<string, PrimitiveTypeCode> _primitiveTypes = new()
        {
            { "System.Void", PrimitiveTypeCode.Void },
            { "void", PrimitiveTypeCode.Void },
            { "System.Boolean", PrimitiveTypeCode.Boolean },
            { "bool", PrimitiveTypeCode.Boolean },
            { "System.Char", PrimitiveTypeCode.Char },
            { "char", PrimitiveTypeCode.Char },
            { "System.SByte", PrimitiveTypeCode.SByte },
            { "sbyte", PrimitiveTypeCode.SByte },
            { "System.Byte", PrimitiveTypeCode.Byte },
            { "byte", PrimitiveTypeCode.Byte },
            { "System.Int16", PrimitiveTypeCode.Int16 },
            { "short", PrimitiveTypeCode.Int16 },
            { "System.UInt16", PrimitiveTypeCode.UInt16 },
            { "ushort", PrimitiveTypeCode.UInt16 },
            { "System.Int32", PrimitiveTypeCode.Int32 },
            { "int", PrimitiveTypeCode.Int32 },
            { "System.UInt32", PrimitiveTypeCode.UInt32 },
            { "uint", PrimitiveTypeCode.UInt32 },
            { "System.Int64", PrimitiveTypeCode.Int64 },
            { "long", PrimitiveTypeCode.Int64 },
            { "System.UInt64", PrimitiveTypeCode.UInt64 },
            { "ulong", PrimitiveTypeCode.UInt64 },
            { "System.Single", PrimitiveTypeCode.Single },
            { "single", PrimitiveTypeCode.Single },
            { "System.Double", PrimitiveTypeCode.Double },
            { "double", PrimitiveTypeCode.Double },
            { "System.String", PrimitiveTypeCode.String },
            { "string", PrimitiveTypeCode.String },
            { "System.IntPtr", PrimitiveTypeCode.IntPtr },
            { "System.UIntPtr", PrimitiveTypeCode.UIntPtr },
            { "System.Object", PrimitiveTypeCode.Object },
            { "object", PrimitiveTypeCode.Object },
            { "duck", PrimitiveTypeCode.Object },
        };

        private void EncodeBasicType(SignatureTypeEncoder enc, IType type)
        {
            if (_primitiveTypes.TryGetValue(type.FullName, out var code))
            {
                enc.PrimitiveType(code);
            }
            else
            {
                enc.Type(LookupType(type), type.IsValueType);
            }
        }

        public EntityHandle LookupMethod(IMethodBase method)
        {
            if (_methodLookup.TryGetValue(method, out var result))
            {
                return result;
            }
            if (method is IMethod m && (m.ConstructedInfo?.FullyConstructed == true || m is GenericConstructedMethod))
            {
                return LookupMethodSpec(m);
            }
            if (method is GenericMappedMethod gmm)
            {
                return LookupMappedMethodSpec(gmm);
            }
            if (method is ExternalMethod em)
            {
                return LookupExternalMethod(em);
            }
            throw new EcmaBuildException($"Unable to lookup handle for method {method.FullName} of entity type {method.GetType()}.");

        }

        private EntityHandle LookupExternalMethod(IMethod method)
        {
            var typeRef = LookupType(method.DeclaringType);
            var name = method.EntityType == EntityType.Constructor ? (method.IsStatic ? ".cctor" : ".ctor") : method.Name;
            var result = AssemblyBuilder.AddMemberReference(typeRef, AssemblyBuilder.GetOrAddString(name), GetMethodSignature(method));
            _methodLookup.Add(method, result);
            return result;
        }

        private EntityHandle LookupMappedMethodSpec(GenericMappedMethod method)
        {
            return LookupExternalMethod(method); //see if this works
        }

        private BlobHandle GetMethodSignature(IMethod method)
        {
            var parent = method.DeclaringType;
            if (parent.ConstructedInfo != null)
            {
                method = (IMethod)parent.ConstructedInfo.UnMap(method);
            }
            var parameters = method.GetParameters();
            var enc = new BlobEncoder(new BlobBuilder());
            var sig = enc.MethodSignature(
                genericParameterCount: method.ConstructedInfo?.GenericArguments?.Length ?? method.GenericInfo?.GenericParameters?.Length ?? 0,
                isInstanceMethod: !method.IsStatic);
            sig.Parameters(parameters.Length, out var retEnc, out var paramEnc);
            if (method.ReturnType == _tss.VoidType)
            {
                retEnc.Void();
            }
            else
            {
                EncodeType(retEnc.Type(), method.ReturnType);
            }
            foreach (var param in parameters)
            {
                if (param.Type.IsByRef)
                {
                    EncodeType(paramEnc.AddParameter().Type(true), param.Type.ElementType);
                }
                else if (param.IsByRef)
                {
                    EncodeType(paramEnc.AddParameter().Type(true), param.Type);
                }
                else
                {
                    EncodeType(paramEnc.AddParameter().Type(false), param.Type);
                }
            }
            // Get blob
            return AssemblyBuilder.GetOrAddBlob(enc.Builder);
        }

        //private int _depth = 0;

        private EntityHandle LookupMethodSpec(IMethod m)
        {
            if (!_methodLookup.TryGetValue(m, out var result))
            {
                //++_depth;
                //if (_depth > 10) System.Diagnostics.Debugger.Break();
                var definition = m.ConstructedInfo.GenericDefinition;
                var defHandle = LookupMethod(definition);
                var args = m.ConstructedInfo.GenericArguments;
                var enc = new BlobEncoder(new BlobBuilder()).MethodSpecificationSignature(args.Length);
                foreach (var arg in args)
                {
                    EncodeType(enc.AddArgument(), arg);
                }
                result = AssemblyBuilder.AddMethodSpecification(defHandle, AssemblyBuilder.GetOrAddBlob(enc.Builder));
                _methodLookup.Add(m, result);
                //--_depth;
            }
            return result;
        }

        public EntityHandle ReserveMethod()
        {
            ++_methods;
            return MetadataTokens.MethodDefinitionHandle(_methods);
        }

        public EntityHandle ReserveMethod(IMethod method)
        {
            EntityHandle result = ReserveMethod();
            _methodLookup[method] = result;
            _reversemethodLookup[result] = method;
            return result;
        }

        private readonly Dictionary<(IType type, string name), EntityHandle> _arrayMethods = new();

        public EntityHandle GetArrayMethod(IType arrayType, string methodName, IType returnType, IType[] parameterTypes)
        {
            (IType type, string name) key = (arrayType, methodName);
            if (!_arrayMethods.TryGetValue(key, out var result))
            {
                var typ = LookupType(arrayType);
                var builder = new BlobEncoder(new BlobBuilder());
                var sig = builder.MethodSignature(SignatureCallingConvention.Default, 0, true);
                sig.Parameters(parameterTypes.Length, out var retEnc, out var paramEnc);
                if (returnType == VoidTypeEntity)
                {
                    retEnc.Void();
                }
                else
                {
                    EncodeType(retEnc.Type(returnType.IsByRef), returnType);
                }
                foreach (var param in parameterTypes)
                {
                    EncodeType(paramEnc.AddParameter().Type(), param);
                }
                result = AssemblyBuilder.AddMemberReference(typ, AssemblyBuilder.GetOrAddString(methodName), AssemblyBuilder.GetOrAddBlob(sig.Builder));
                _arrayMethods[key] = result;
            }
            return result;
        }

        public IMethodBase MethodOf(IType typ, string name, params IType[] args)
        {
            var method = typ.GetMembers().OfType<IMethodBase>().Single(m => m.Name == name && ParamsMatch(m.GetParameters(), args));
            return method;
        }

        public IMethodBase MethodOf(Type typ, string name, params Type[] args) =>
            MethodOf(_tss.Map(typ), name, args.Select(_tss.Map).ToArray());

        public IMethodBase MethodOf<T>(string name, params Type[] args) => MethodOf(typeof(T), name, args);

        public IMethodBase MethodOf<T>(string name, params IType[] args) => MethodOf(_tss.Map(typeof(T)), name, args);

        public IConstructor ConstructorOf<T>(params Type[] args) => (IConstructor)MethodOf<T>("constructor", args);

        public IConstructor ConstructorOf<T>(params IType[] args) => (IConstructor)MethodOf<T>("constructor", args);

        private static bool ParamsMatch(IParameter[] parameters, IType[] args)
        {
            if (parameters.Length != args.Length)
                return false;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].Type != args[i].Type)
                    return false;
            }
            return true;
        }

        internal EntityHandle FieldOf(IType type, string fieldName)
        {
            var field = type.GetMembers().OfType<IField>().Single(f => f.Name == fieldName);
            return LookupField(field);
        }

        internal EntityHandle LookupField(IField field)
        {
            if (!_fieldLookup.TryGetValue(field, out var result))
            {
                var type = field.DeclaringType;
                var sigField = type.ConstructedInfo == null
                    ? field
                    : type.ConstructedInfo.GenericDefinition.GetMembers().OfType<IField>().Single(f => f.Name == field.Name);
                var sig = new BlobEncoder(new BlobBuilder());
                EncodeType(sig.FieldSignature(), sigField.Type);
                var typeHandle = LookupType(type);
                result = AssemblyBuilder.AddMemberReference(
                    typeHandle,
                    AssemblyBuilder.GetOrAddString(field.Name),
                    AssemblyBuilder.GetOrAddBlob(sig.Builder));
                _fieldLookup.Add(field, result);
            }
            return result;
        }

        public EntityHandle ReserveField(IField field)
        {
            ++_fields;
            var result = MetadataTokens.FieldDefinitionHandle(_fields);
            _fieldLookup.Add(field, result);
            return result;
        }

        public EntityHandle ReserveEvent(IEvent ev)
        {
            ++_events;
            var result = MetadataTokens.EventDefinitionHandle(_events);
            _eventLookup.Add(ev, result);
            return result;
        }

        internal EntityHandle ReserveProperty(IProperty pr)
        {
            ++_props;
            var result = MetadataTokens.PropertyDefinitionHandle(_props);
            _propLookup.Add(pr, result);
            return result;
        }

        internal EntityHandle ReserveParameter(IParameter pa)
        {
            ++_params;
            var result = MetadataTokens.ParameterHandle(_params);
            //_paramLookup.Add(pa, result);
            return result;
        }

        internal object GetExpressionValue(Expression expr, IType expectedType) => _getValue(expectedType ?? expr.ExpressionType, expr);

        internal CustomAttributeHandle SetCustomAttribute(IBuilder builder, AttributeBuilder attr)
        {
            if (attr.IsSpecial && attr.HandleSpecialAttribute(builder))
            {
                return default;
            } else return SetCustomAttribute(builder.Handle, attr);
        }

        internal CustomAttributeHandle SetCustomAttribute(EntityHandle handle, AttributeBuilder attr)
        {
            if (attr.Handle.IsNil)
            {
                attr.Build();
            }
            return AssemblyBuilder.AddCustomAttribute(handle, attr.ConstructorHandle, attr.Handle);
        }

        internal MethodSpecificationHandle ConstructMethodOfGenericType(IMethod baseMethod, IType type)
        {
            var constructedMethod = (IMethod)baseMethod.DeclaringType.GenericInfo.ConstructType(type).ConstructedInfo.Map(baseMethod);
            return (MethodSpecificationHandle)LookupMethod(constructedMethod);
        }

        internal void RegisterGenericParameter(EntityHandle parent, GenericTypeParameterBuilder builder)
        {
            _genParamBuilders.Add((parent, builder));
        }

        internal void BuildGenericParameters()
        {
            foreach (var (handle, builder) in _genParamBuilders.OrderBy(p => CodedIndex.TypeOrMethodDef(p.parent)))
            {
                var idx = CodedIndex.TypeOrMethodDef(handle);
                builder.Build();
            }
        }

        internal void RegisterNestedType(EntityHandle parent, TypeBuilder builder)
        {
            _typeBuilders.Add((parent, builder));
        }

        internal void BuildNestedTypes()
        {
            foreach (var (handle, builder) in _typeBuilders.OrderBy(p => CodedIndex.TypeDefOrRef(p.builder.Handle)))
            {
                var idx = CodedIndex.TypeDefOrRef(handle);
                AssemblyBuilder.AddNestedType((TypeDefinitionHandle)builder.Handle, (TypeDefinitionHandle)handle);
            }
        }

        internal DocumentHandle LookupDocument(string name)
        {
            // Normalize filename, just in case
            name = Path.GetFullPath(name);
            if (!_documentLookup.TryGetValue(name, out var result))
            {
                var handle = DebugBuilder.GetOrAddDocumentName(name);
                result = DebugBuilder.AddDocument(handle, default, default, _booLangGuidHandle);
                _documentLookup[name] = result;
            }
            return result;
        }
    }
}
