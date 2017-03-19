#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//	   * Redistributions of source code must retain the above copyright notice,
//	   this list of conditions and the following disclaimer.
//	   * Redistributions in binary form must reproduce the above copyright notice,
//	   this list of conditions and the following disclaimer in the documentation
//	   and/or other materials provided with the distribution.
//	   * Neither the name of Rodrigo B. de Oliveira nor the names of its
//	   contributors may be used to endorse or promote products derived from this
//	   software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Security;

using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps.EmitCCI;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Environments;
using Boo.Lang.Runtime;
using Microsoft.Cci.Immutable;
using AssemblyName = System.Reflection.AssemblyName;
using Attribute = Boo.Lang.Compiler.Ast.Attribute;
using IArrayType = Boo.Lang.Compiler.TypeSystem.IArrayType;
using IAssemblyReference = Microsoft.Cci.IAssemblyReference;
using Module = Boo.Lang.Compiler.Ast.Module;


namespace Boo.Lang.Compiler.Steps
{
    internal sealed class LoopInfoCci
    {
        public ILGeneratorLabel BreakLabel;

        public ILGeneratorLabel ContinueLabel;

        public int TryBlockDepth;

        public LoopInfoCci(ILGeneratorLabel breakLabel, ILGeneratorLabel continueLabel, int tryBlockDepth)
        {
            BreakLabel = breakLabel;
            ContinueLabel = continueLabel;
            TryBlockDepth = tryBlockDepth;
        }
    }

    public class EmitAssemblyCci : AbstractFastVisitorCompilerStep
	{
        private INameTable _nameTable;
        private static PeReader.DefaultHost _host;
        private IAssembly _coreAssembly;

        private Assembly _asmBuilder;
	    private Microsoft.Cci.MutableCodeModel.Module _moduleBuilder;
	    private NamespaceTypeDefinition _moduleClass;

        // IL generation state
        private ILGenerator _il;
        private Method _method;       //current method
        private int _returnStatements;//number of return statements in current method
        private bool _hasLeaveWithStoredValue;//has a explicit return inside a try block (a `leave')
        private bool _returnImplicit; //method ends with an implicit return
        private ILGeneratorLabel _returnLabel;   //label for `ret'
        private ILGeneratorLabel _implicitLabel; //label for implicit return (with default value)
        private ILGeneratorLabel _leaveLabel;    //label to load the stored return value (because of a `leave')
        private IType _returnType;
        private int _tryBlock; // are we in a try block?
        private bool _checked = true;
        private bool _rawArrayIndexing = false;
        private bool _perModuleRawArrayIndexing = false;

        private readonly Dictionary<IType, ITypeReference> _typeCache = new Dictionary<IType, ITypeReference>();
        private readonly List<Method> _moduleConstructorMethods = new List<Method>();

        // keeps track of types on the IL stack
        private readonly Stack<IType> _types = new Stack<IType>();

        private readonly Stack<LoopInfoCci> _loopInfoStack = new Stack<LoopInfoCci>();

        private readonly AttributeCollection _assemblyAttributes = new AttributeCollection();

        private LoopInfoCci _currentLoopInfo;

        private BooSourceDocument _currentDocument;

	    public static void Reset() //necessary for automated testing; should not be called otherwise
	    {
	        _host = null;
	        CompilerContext.ClearCodegenMap();
	    }

        private void EnterLoop(ILGeneratorLabel breakLabel, ILGeneratorLabel continueLabel)
        {
            _loopInfoStack.Push(_currentLoopInfo);
            _currentLoopInfo = new LoopInfoCci(breakLabel, continueLabel, _tryBlock);
        }

        private bool InTryInLoop()
        {
            return _tryBlock > _currentLoopInfo.TryBlockDepth;
        }

        private void LeaveLoop()
        {
            _currentLoopInfo = _loopInfoStack.Pop();
        }

        private void PushType(IType type)
        {
            _types.Push(type);
        }

        private void PushBool()
        {
            PushType(TypeSystemServices.BoolType);
        }

        private void PushVoid()
        {
            PushType(TypeSystemServices.VoidType);
        }

        private IType PopType()
        {
            return _types.Pop();
        }

        private IType PeekTypeOnStack()
        {
            return (_types.Count != 0) ? _types.Peek() : null;
        }

        public override void Run()
        {
            if (Errors.Count > 0)
                return;

            try
            {
                GatherAssemblyAttributes();
                SetUpAssembly();

                DefineTypes();

                DefineResources();
                DefineAssemblyAttributes();
                DefineEntryPoint();
                DefineModuleConstructor();

                // Define the unmanaged version information resource, which 
                // contains the attribute informaion applied earlier
                // MW: Commenting out as this appears to create a resource containing no actual version info.
                // https://github.com/dotnet/coreclr/blob/f8b8b6ab80f1c5b30cc04676ca2e084ce200161e/src/mscorlib/src/System/Reflection/Emit/AssemblyBuilder.cs#L1418
                // _asmBuilder.DefineVersionInfoResource();
            }
            finally
            {

                Cleanup();
            }
        }

	    private void Cleanup()
	    {
            _typeCache.Clear();
            _moduleConstructorMethods.Clear();
            _assemblyAttributes.Clear();
            _defaultValueHolders.Clear();
            _packedArrays.Clear();
            _builders.Clear();
            _namespaceMap.Clear();
            _nullableHasValue.Clear();
	    }

        private void GatherAssemblyAttributes()
        {
            foreach (var module in CompileUnit.Modules)
                foreach (var attribute in module.AssemblyAttributes)
                    _assemblyAttributes.Add(attribute);
        }

        private void DefineTypes()
        {
            if (CompileUnit.Modules.Count == 0)
                return;

            var types = CollectTypes();
            foreach (var type in types)
                DefineType(type);

            foreach (var type in types)
                DefineGenericParameters(type);

            foreach (var type in types)
                DefineTypeMembers(type);

            foreach (var module in CompileUnit.Modules)
                OnModule(module);

            EmitAttributes();
        }

        private sealed class AttributeEmitVisitor : FastDepthFirstVisitor
        {
            private readonly EmitAssemblyCci _emitter;

            public AttributeEmitVisitor(EmitAssemblyCci emitter)
            {
                _emitter = emitter;
            }

            public override void OnField(Field node)
            {
                _emitter.EmitFieldAttributes(node);
            }

            public override void OnEnumMember(EnumMember node)
            {
                _emitter.EmitFieldAttributes(node);
            }

            public override void OnEvent(Event node)
            {
                _emitter.EmitEventAttributes(node);
            }

            public override void OnProperty(Property node)
            {
                Visit(node.Getter);
                Visit(node.Setter);
                _emitter.EmitPropertyAttributes(node);
            }

            public override void OnConstructor(Constructor node)
            {
                Visit(node.Parameters);
                _emitter.EmitConstructorAttributes(node);
            }

            public override void OnMethod(Method node)
            {
                Visit(node.Parameters);
                _emitter.EmitMethodAttributes(node);
            }

            public override void OnParameterDeclaration(ParameterDeclaration node)
            {
                _emitter.EmitParameterAttributes(node);
            }

            public override void OnClassDefinition(ClassDefinition node)
            {
                base.OnClassDefinition(node);
                _emitter.EmitTypeAttributes(node);
            }

            public override void OnInterfaceDefinition(InterfaceDefinition node)
            {
                base.OnInterfaceDefinition(node);
                _emitter.EmitTypeAttributes(node);
            }

            public override void OnEnumDefinition(EnumDefinition node)
            {
                base.OnEnumDefinition(node);
                _emitter.EmitTypeAttributes(node);
            }
        }

        private void EmitAttributes<T>(INodeWithAttributes node, T member, System.Action<T, CustomAttribute> setter)
        {
            foreach (Attribute attribute in node.Attributes)
                setter(member, GetCustomAttributeBuilder(attribute));
        }

        private static void SetCustomAttribute(NamedTypeDefinition member, CustomAttribute attribute)
        {
            if (member.Attributes == null)
                member.Attributes = new System.Collections.Generic.List<ICustomAttribute>();
            member.Attributes.Add(attribute);
        }

        private static void SetCustomAttribute(TypeDefinitionMember member, CustomAttribute attribute)
	    {
	        if (member.Attributes == null)
                member.Attributes = new System.Collections.Generic.List<ICustomAttribute>();
            member.Attributes.Add(attribute);
	    }

        private static void SetCustomAttribute(ParameterDefinition member, CustomAttribute attribute)
        {
            if (member.Attributes == null)
                member.Attributes = new System.Collections.Generic.List<ICustomAttribute>();
            member.Attributes.Add(attribute);
        }

        private void EmitPropertyAttributes(Property node)
        {
            var builder = GetPropertyBuilder(node);
            EmitAttributes(node, builder, SetCustomAttribute);
        }

        private void EmitParameterAttributes(ParameterDeclaration node)
        {
            ParameterDefinition builder = (ParameterDefinition)GetBuilder(node);
            EmitAttributes(node, builder, SetCustomAttribute);
        }

        private void EmitEventAttributes(Event node)
        {
            var builder = (TypeDefinitionMember)GetBuilder(node);
            EmitAttributes(node, builder, SetCustomAttribute);
        }

        private void EmitConstructorAttributes(Constructor node)
        {
            var builder = (TypeDefinitionMember)GetBuilder(node);
            EmitAttributes(node, builder, SetCustomAttribute);
        }

        private void EmitMethodAttributes(Method node)
        {
            var builder = (TypeDefinitionMember)GetBuilder(node);
            EmitAttributes(node, builder, SetCustomAttribute);
        }

        private void EmitTypeAttributes(TypeDefinition node)
        {
            var builder = (NamedTypeDefinition)GetBuilder(node);
            EmitAttributes(node, builder, SetCustomAttribute);
        }

        private void EmitFieldAttributes(TypeMember node)
        {
            var builder = (TypeDefinitionMember)GetBuilder(node);
            EmitAttributes(node, builder, SetCustomAttribute);
        }

        private void EmitAttributes()
        {
            var visitor = new AttributeEmitVisitor(this);
            foreach (Module module in CompileUnit.Modules)
                module.Accept(visitor);
        }

        private List<TypeDefinition> CollectTypes()
        {
            var types = new List<TypeDefinition>();
            foreach (Module module in CompileUnit.Modules)
                CollectTypes(types, module.Members);
            return types;
        }

        private void CollectTypes(List<TypeDefinition> types, TypeMemberCollection members)
        {
            foreach (var member in members)
            {
                switch (member.NodeType)
                {
                    case NodeType.InterfaceDefinition:
                    case NodeType.ClassDefinition:
                        {
                            var typeDefinition = ((TypeDefinition)member);
                            types.Add(typeDefinition);
                            CollectTypes(types, typeDefinition.Members);
                            break;
                        }
                    case NodeType.EnumDefinition:
                        {
                            types.Add((TypeDefinition)member);
                            break;
                        }
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _asmBuilder = null;
            _il = null;
            _returnStatements = 0;
            _hasLeaveWithStoredValue = false;
            _returnImplicit = false;
            _returnType = null;
            _tryBlock = 0;
            _checked = true;
            _rawArrayIndexing = false;
            _types.Clear();
            _typeCache.Clear();
            _builders.Clear();
            _assemblyAttributes.Clear();
            _defaultValueHolders.Clear();
            _packedArrays.Clear();
        }

        public override void OnAttribute(Attribute node)
        {
        }

        public override void OnModule(Module module)
        {
            _perModuleRawArrayIndexing = AstAnnotations.IsRawIndexing(module);
            _checked = AstAnnotations.IsChecked(module, Parameters.Checked);
            _currentDocument = (BooSourceDocument)module[AstAnnotations.CCI_DOC_KEY];
            Visit(module.Members);
        }

        public override void OnEnumDefinition(EnumDefinition node)
        {
            var typeBuilder = GetTypeBuilder(node);
            typeBuilder.IsValueType = true;
            foreach (EnumMember member in node.Members)
            {
                var field = new FieldDefinition
                {
                    Name = _nameTable.GetNameFor(member.Name),
                    ContainingTypeDefinition = typeBuilder,
                    Visibility = TypeMemberVisibility.Public,
                    IsStatic = true,
                    IsCompileTimeConstant = true,
                    InternFactory = _host.InternFactory,
                    CompileTimeValue = InitializerValueOf(member, node),
                    Type = typeBuilder
                };
                SetBuilder(member, field);
                typeBuilder.Fields.Add(field);
            }
        }

        private ITypeDefinition GetNestedTypeReference(Type value)
        {
            var declaringType = GetTypeReference(value.DeclaringType);
            if (value.IsGenericParameter)
            {
                if (value.DeclaringType != null)
                    return new GenericTypeParameter
                    {
                        InternFactory = _host.InternFactory,
                        Name = _nameTable.GetNameFor(value.Name),
                        DefiningType = GetTypeReference(value.DeclaringType),
                        Index = (ushort)value.GenericParameterPosition
                    };
            }
            ushort genArgCount;
            var specializedBase = 0;
            if (value.ContainsGenericParameters)
            {
                genArgCount = (ushort)value.GetGenericArguments()
                    .Select(g => g.Name)
                    .Except(value.DeclaringType.GetGenericArguments().Select(g => g.Name))
                    .Count();
                if (declaringType.IsGeneric)
                {
                    specializedBase = declaringType.GenericParameterCount;
                    declaringType = GetSelfMappedClass(declaringType);
                }
            }
            else genArgCount = 0;
            var result = new Microsoft.Cci.Immutable.NestedTypeReference(_host, declaringType, _nameTable.GetNameFor(value.Name), 
                genArgCount, value.IsEnum, value.IsValueType, true).ResolvedType;
            if (genArgCount == 0 && value.IsGenericType && specializedBase == 0)
            {
                return GenericTypeInstance.GetGenericTypeInstance(result,
                    value.GetGenericArguments().Select(GetTypeReference),
                    _host.InternFactory);
            }
            return result;
        }

	    private ITypeDefinition GetTypeReference(Type value)
	    {
	        if (value.IsByRef)
	        {
	            var referredType = value.GetElementType();
	            return GetTypeReference(referredType);
	        }
            if (value.IsArray)
            {
                var elementType = GetTypeReference(value.GetElementType());
                var rank = value.GetArrayRank();
                var sysArrayType = rank == 1
                    ? (IArrayTypeReference)new VectorTypeReference
                    {
                        InternFactory = _host.InternFactory,
                        ElementType = elementType,
                        Rank = 1
                    }
                    : (IArrayTypeReference)new MatrixTypeReference
                    {
                        InternFactory = _host.InternFactory,
                        ElementType = elementType,
                        Rank = (uint)rank
                    };
                return sysArrayType.ResolvedType;

            }
	        if (value.IsNested)
                return GetNestedTypeReference(value);
            if (value.IsGenericType && (!value.ContainsGenericParameters || value.FullName == null))
                return GetGenericTypeReference(value);
            if (value.IsPointer)
                return PointerType.GetPointerType(GetTypeReference(value.GetElementType()), _host.InternFactory);

	        var reflectAsm = value.Assembly;
	        var name = reflectAsm.GetName();
	        var asm = LoadAssembly(reflectAsm, name);
            if (value.IsGenericType)
                return UnitHelper.FindType(_nameTable, asm, NonGenericName(value.FullName), value.GetGenericArguments().Length);
	        return UnitHelper.FindType(_nameTable, asm, value.FullName);
	    }

        private readonly Dictionary<System.Reflection.Assembly, IAssembly> _asmCache = new Dictionary<System.Reflection.Assembly, IAssembly>();

	    private IAssembly LoadAssembly(System.Reflection.Assembly reflectAsm, AssemblyName name)
	    {
	        IAssembly result;
	        if (!_asmCache.TryGetValue(reflectAsm, out result))
	        {
	            result = _host.LoadAssembly(new AssemblyIdentity(
	                _nameTable.GetNameFor(reflectAsm.FullName),
	                name.CultureInfo.Name,
	                name.Version,
	                name.GetPublicKeyToken(),
	                reflectAsm.Location));
                _asmCache.Add(reflectAsm, result);
	        }
	        return result;
	    }

	    private static string NonGenericName(string value)
	    {
	        var idx = value.IndexOf("`", StringComparison.Ordinal);
	        return value.Substring(0, idx);
	    
        }

	    private ITypeDefinition GetGenericTypeReference(Type value)
	    {
	        var baseType = value.GetGenericTypeDefinition();
	        var baseTypeRef = GetTypeReference(baseType);
	        var args = value.GetGenericArguments().Select(GetTypeReference).ToArray();
            return GenericTypeInstance.GetGenericTypeInstance((INamedTypeDefinition)baseTypeRef, args, _host.InternFactory);
	    }

	    private ITypeDefinition GetTypeReference<T>()
	    {
	        return GetTypeReference(typeof(T));
	    }

        private IMetadataConstant InitializerValueOf(EnumMember enumMember, EnumDefinition enumType)
        {
            return new MetadataConstant
            {
                Value = Convert.ChangeType(((IntegerLiteralExpression) enumMember.Initializer).Value,
                    GetEnumUnderlyingType(enumType)),
                Type = GetTypeReference(GetEnumUnderlyingType(enumType))
            };
        }

        public override void OnArrayTypeReference(Ast.ArrayTypeReference node)
        {
        }

        public override void OnClassDefinition(ClassDefinition node)
        {
            EmitTypeDefinition(node);
        }

        public override void OnField(Field node)
        {
            FieldDefinition builder = GetFieldBuilder(node);
            
            if (builder.IsCompileTimeConstant)
            {
                builder.CompileTimeValue = GetInternalFieldStaticValue((InternalField)node.Entity);
            }
        }

        public override void OnInterfaceDefinition(InterfaceDefinition node)
        {
            NamedTypeDefinition builder = GetTypeBuilder(node);
            foreach (var baseType in node.BaseTypes)
            {
                if (builder.Interfaces == null)
                    builder.Interfaces = new System.Collections.Generic.List<ITypeReference>();
                builder.Interfaces.Add(GetSystemType(baseType));
            }
        }

        public override void OnMacroStatement(MacroStatement node)
        {
            NotImplemented(node, "Unexpected macro: " + node.ToCodeString());
        }

        public override void OnCallableDefinition(CallableDefinition node)
        {
            NotImplemented(node, "Unexpected callable definition!");
        }

        private void EmitTypeDefinition(TypeDefinition node)
        {
            NamedTypeDefinition current = GetTypeBuilder(node);
            EmitBaseTypesAndAttributes(node, current);
            Visit(node.Members);
        }

        public override void OnMethod(Method method)
        {
            if (method.IsRuntime) return;
            if (IsPInvoke(method)) return;

            MethodDefinition methodBuilder = GetMethodBuilder(method);
            DefineExplicitImplementationInfo(method);

            EmitMethod(method, methodBuilder);
            if (method.Name.StartsWith("$module_ctor"))
            {
                _moduleConstructorMethods.Add(method);
            }
        }

        private void DefineExplicitImplementationInfo(Method method)
        {
            if (null == method.ExplicitInfo)
                return;

            var ifaceMethod = (IMethod)method.ExplicitInfo.Entity;
            var ifaceInfo = GetMethodInfo(ifaceMethod);
            var implInfo = GetMethodInfo((IMethod)method.Entity);

            var typeBuilder = GetTypeBuilder(method.DeclaringType);
            if (typeBuilder.ExplicitImplementationOverrides == null)
                typeBuilder.ExplicitImplementationOverrides = new System.Collections.Generic.List<IMethodImplementation>();
            typeBuilder.ExplicitImplementationOverrides.Add(new MethodImplementation
            {
                ContainingType = typeBuilder,
                ImplementedMethod = ifaceInfo,
                ImplementingMethod = implInfo
            });
        }

        private System.Collections.Generic.List<ILocalDefinition> _locals;

        private void EmitMethod(Method method, MethodDefinition builder)
        {
            var generator = new ILGenerator(_host, builder);
            _il = generator;
            _method = method;

            var oldLocals = _locals;
            _locals = new System.Collections.Generic.List<ILocalDefinition>();
            try
            {
                DefineLabels(method);
                Visit(method.Locals);

                BeginMethodBody(GetEntity(method).ReturnType);
                Visit(method.Body);
                EndMethodBody(method);
                var body = new ILGeneratorMethodBody(generator, true, IlGeneratorHelper.MaxStackSize(generator),
                    generator.Method, _locals, Enumerable.Empty<ITypeDefinition>());
                builder.Body = body;
            }
            finally
            {
                _locals = oldLocals;
            }
        }

        private void BeginMethodBody(IType returnType)
        {
            _defaultValueHolders.Clear();

            _returnType = returnType;
            _returnStatements = 0;
            _returnImplicit = IsVoid(returnType);
            _hasLeaveWithStoredValue = false;

            //we may not actually use (any/all of) them, but at least they're ready
            _returnLabel = new ILGeneratorLabel();
            _leaveLabel = new ILGeneratorLabel();
            _implicitLabel = new ILGeneratorLabel();
        }

        private void EndMethodBody(Method method)
        {
            if (!_returnImplicit)
                _returnImplicit = !AstUtil.AllCodePathsReturnOrRaise(method.Body);

            //At most a method epilogue contains 3 independent load instructions:
            //1) load of the value of an actual return (emitted elsewhere and branched to _returnLabel)
            //2) load of a default value (implicit returns [e.g return without expression])
            //3) load of the `leave' stored value

            bool hasDefaultValueReturn = _returnImplicit && !IsVoid(_returnType);
            if (hasDefaultValueReturn)
            {
                if (_returnStatements == -1) //emit branch only if instructed to do so (-1)
                    _il.Emit(OperationCode.Br_S, _returnLabel);

                //load default return value for implicit return
                _il.MarkLabel(_implicitLabel);
                EmitDefaultValue(_returnType);
                PopType();
            }

            if (_hasLeaveWithStoredValue)
            {
                if (hasDefaultValueReturn || _returnStatements == -1)
                    _il.Emit(OperationCode.Br_S, _returnLabel);

                //load the stored return value and `ret'
                _il.MarkLabel(_leaveLabel);
                _il.Emit(OperationCode.Ldloc, GetDefaultValueHolder(_returnType));
            }

            if (_returnImplicit || _returnStatements != 0)
            {
                _il.MarkLabel(_returnLabel);
                _il.Emit(OperationCode.Ret);
            }
        }

        private bool IsPInvoke(Method method)
        {
            return GetEntity(method).IsPInvoke;
        }

        public override void OnBlock(Block block)
        {
            var currentChecked = _checked;
            _checked = AstAnnotations.IsChecked(block, currentChecked);

            var currentArrayIndexing = _rawArrayIndexing;
            _rawArrayIndexing = _perModuleRawArrayIndexing || AstAnnotations.IsRawIndexing(block);

            Visit(block.Statements);

            _rawArrayIndexing = currentArrayIndexing;
            _checked = currentChecked;
        }

        private void DefineLabels(Method method)
        {
            foreach (var label in LabelsOn(method))
                label.LabelCci = new ILGeneratorLabel();
        }

        private static IEnumerable<InternalLabel> LabelsOn(Method method)
        {
            return ((InternalMethod)method.Entity).Labels;
        }

        public override void OnConstructor(Constructor constructor)
        {
            if (constructor.IsRuntime) return;

            MethodDefinition builder = GetConstructorBuilder(constructor);
            EmitMethod(constructor, builder);
        }

        private LocalDefinition DefineLocal(string name, ITypeReference type, bool isPinned = false)
        {
            if (name == null)
                name = "V_" + _locals.Count.ToString();
            var result = new LocalDefinition
            {
                Name = _nameTable.GetNameFor(name),
                Type = type,
                IsPinned = isPinned
            };
            _locals.Add(result);
            return result;
        }

        public override void OnLocal(Local local)
        {
            InternalLocal info = GetInternalLocal(local);
            info.LocalDefinition = DefineLocal(local.Name, GetSystemType(local), info.Type.IsPointer);
        }

        public override void OnForStatement(ForStatement node)
        {
            NotImplemented("ForStatement");
        }

        public override void OnReturnStatement(ReturnStatement node)
        {
            EmitDebugInfo(node);

            var retOpCode = _tryBlock > 0 ? OperationCode.Leave : OperationCode.Br;
            var label = _returnLabel;

            var expression = node.Expression;
            if (expression != null)
            {
                ++_returnStatements;

                LoadExpressionWithType(_returnType, expression);

                if (retOpCode == OperationCode.Leave)
                {
                    //`leave' clears the stack, so we have to store return value temporarily
                    //we can use a default value holder for that since it won't be read afterwards
                    //of course this is necessary only if return type is not void
                    LocalDefinition temp = GetDefaultValueHolder(_returnType);
                    _il.Emit(OperationCode.Stloc, temp);
                    label = _leaveLabel;
                    _hasLeaveWithStoredValue = true;
                }
            }
            else if (_returnType != TypeSystemServices.VoidType)
            {
                _returnImplicit = true;
                label = _implicitLabel;
            }

            if (_method.Body.LastStatement != node)
                _il.Emit(retOpCode, label);
            else if (null != expression)
                _returnStatements = -1; //instruct epilogue to branch last ret only if necessary
        }

        private void LoadExpressionWithType(IType expectedType, Expression expression)
        {
            Visit(expression);
            EmitCastIfNeeded(expectedType, PopType());
        }

        public override void OnRaiseStatement(RaiseStatement node)
        {
            EmitDebugInfo(node);
            if (node.Exception == null)
            {
                _il.Emit(OperationCode.Rethrow);
            }
            else
            {
                Visit(node.Exception); PopType();
                _il.Emit(OperationCode.Throw);
            }
        }

        private ILGeneratorLabel _exceptionExitPoint;

        private void EmitTryBlock(Action action)
        {
            ++_tryBlock;
            _il.BeginTryBody();
            var oldExit = _exceptionExitPoint;
            _exceptionExitPoint = new ILGeneratorLabel();
            try
            {
                action();
            }
            finally
            {
                _il.EndTryBody();
                --_tryBlock;
                _il.MarkLabel(_exceptionExitPoint);
                _exceptionExitPoint = oldExit;
            }
        }

	    private void EmitTryBlockIf(bool condition, Action action)
	    {
	        if (condition)
	        {
	            EmitTryBlock(action);
                _il.Emit(OperationCode.Leave, _exceptionExitPoint);
	        }
	        else action();
	    }

        public override void OnTryStatement(TryStatement node)
        {
            EmitTryBlock(() =>
            {
                EmitTryBlockIf(node.EnsureBlock != null && (node.FailureBlock != null || node.ExceptionHandlers.Count > 0), () =>
                {
                    EmitTryBlockIf(node.FailureBlock != null && node.ExceptionHandlers.Count > 0, () =>
                    {
                        Visit(node.ProtectedBlock);
                        _il.Emit(OperationCode.Leave, _exceptionExitPoint);

                        if (node.ExceptionHandlers.Count > 0)
                        {
                            Visit(node.ExceptionHandlers);
                        }
                    });

                    if (node.FailureBlock != null)
                    {
                        _il.BeginFaultBlock();
                        Visit(node.FailureBlock);
                        _il.Emit(OperationCode.Endfinally);
                    }
                });

                if (node.EnsureBlock != null)
                {
                    _il.BeginFinallyBlock();
                    Visit(node.EnsureBlock);
                    _il.Emit(OperationCode.Endfinally);
                }
            });
        }

        public override void OnExceptionHandler(ExceptionHandler node)
        {
            if ((node.Flags & ExceptionHandlerFlags.Filter) == ExceptionHandlerFlags.Filter)
            {
                _il.BeginFilterBlock();

                ILGeneratorLabel endLabel = new ILGeneratorLabel();

                // If the filter is not untyped, then test the exception type
                // before testing the filter condition
                if ((node.Flags & ExceptionHandlerFlags.Untyped) == ExceptionHandlerFlags.None)
                {
                    ILGeneratorLabel filterCondition = new ILGeneratorLabel();

                    // Test the type of the exception.
                    _il.Emit(OperationCode.Isinst, GetSystemType(node.Declaration.Type));

                    // Duplicate it.  If it is null, then it will be used to
                    // skip the filter.
                    Dup();

                    // If the exception is of the right type, branch
                    // to test the filter condition.
                    _il.Emit(OperationCode.Brtrue_S, filterCondition);

                    // Otherwise, clean up the stack and prepare the stack
                    // to skip the filter.
                    EmitStoreOrPopException(node);

                    _il.Emit(OperationCode.Ldc_I4_0);
                    _il.Emit(OperationCode.Br, endLabel);
                    _il.MarkLabel(filterCondition);

                }
                else if ((node.Flags & ExceptionHandlerFlags.Anonymous) == ExceptionHandlerFlags.None)
                {
                    // Cast the exception to the default except type
                    _il.Emit(OperationCode.Isinst, GetSystemType(node.Declaration.Type));
                }

                EmitStoreOrPopException(node);

                // Test the condition and convert to boolean if needed.
                node.FilterCondition.Accept(this);
                PopType();
                EmitToBoolIfNeeded(node.FilterCondition);

                // If the type is right and the condition is true,
                // proceed with the handler.
                _il.MarkLabel(endLabel);
                _il.Emit(OperationCode.Ldc_I4_0);
                _il.Emit(OperationCode.Cgt_Un);

                _il.BeginFilterBody();
            }
            else
            {
                // Begin a normal catch block of the appropriate type.
                _il.BeginCatchBlock(GetSystemType(node.Declaration.Type));

                // Clean up the stack or store the exception if not anonymous.
                EmitStoreOrPopException(node);
            }

            Visit(node.Block);
            _il.Emit(OperationCode.Leave, _exceptionExitPoint);
        }

        private void EmitStoreOrPopException(ExceptionHandler node)
        {
            if ((node.Flags & ExceptionHandlerFlags.Anonymous) == ExceptionHandlerFlags.None)
            {
                _il.Emit(OperationCode.Stloc, GetLocalBuilder(node.Declaration));
            }
            else
            {
                _il.Emit(OperationCode.Pop);
            }
        }

        public override void OnUnpackStatement(UnpackStatement node)
        {
            NotImplemented("Unpacking");
        }

        public override void OnExpressionStatement(ExpressionStatement node)
        {
            EmitDebugInfo(node);

            base.OnExpressionStatement(node);

            // if the type of the inner expression is not
            // void we need to pop its return value to leave
            // the stack sane
            DiscardValueOnStack();
        }

        private void DiscardValueOnStack()
        {
            if (!IsVoid(PopType()))
                _il.Emit(OperationCode.Pop);
        }

        bool IsVoid(IType type)
        {
            return type == TypeSystemServices.VoidType;
        }

        public override void OnUnlessStatement(UnlessStatement node)
        {
            ILGeneratorLabel endLabel = new ILGeneratorLabel();
            EmitDebugInfo(node);
            EmitBranchTrue(node.Condition, endLabel);
            node.Block.Accept(this);
            _il.MarkLabel(endLabel);
        }

        private void OnSwitch(MethodInvocationExpression node)
        {
            var args = node.Arguments;
            LoadExpressionWithType(TypeSystemServices.IntType, args[0]);
            _il.Emit(OperationCode.Switch, args.Skip(1).Select(LabelFor).ToArray());

            PushVoid();
        }

        private static ILGeneratorLabel LabelFor(Expression expression)
        {
            return ((InternalLabel)expression.Entity).LabelCci;
        }

        public override void OnGotoStatement(GotoStatement node)
        {
            EmitDebugInfo(node);

            InternalLabel label = (InternalLabel)GetEntity(node.Label);
            int gotoDepth = AstAnnotations.GetTryBlockDepth(node);
            int targetDepth = AstAnnotations.GetTryBlockDepth(label.LabelStatement);

            if (targetDepth == gotoDepth)
            {
                _il.Emit(OperationCode.Br, label.LabelCci);
            }
            else
            {
                _il.Emit(OperationCode.Leave, label.LabelCci);
            }
        }

        public override void OnLabelStatement(LabelStatement node)
        {
            EmitDebugInfo(node);
            _il.MarkLabel(((InternalLabel)node.Entity).LabelCci);
        }

        public override void OnConditionalExpression(ConditionalExpression node)
        {
            var type = GetExpressionType(node);

            var endLabel = new ILGeneratorLabel();

            EmitBranchFalse(node.Condition, endLabel);
            LoadExpressionWithType(type, node.TrueValue);

            var elseEndLabel = new ILGeneratorLabel();
            _il.Emit(OperationCode.Br, elseEndLabel);
            _il.MarkLabel(endLabel);

            endLabel = elseEndLabel;
            LoadExpressionWithType(type, node.FalseValue);

            _il.MarkLabel(endLabel);

            PushType(type);
        }

        public override void OnIfStatement(IfStatement node)
        {
            ILGeneratorLabel endLabel = new ILGeneratorLabel();

            EmitDebugInfo(node);
            EmitBranchFalse(node.Condition, endLabel);

            node.TrueBlock.Accept(this);
            if (null != node.FalseBlock)
            {
                ILGeneratorLabel elseEndLabel = new ILGeneratorLabel();
                if (!node.TrueBlock.EndsWith<ReturnStatement>() && !node.TrueBlock.EndsWith<RaiseStatement>())
                    _il.Emit(OperationCode.Br, elseEndLabel);
                _il.MarkLabel(endLabel);

                endLabel = elseEndLabel;
                node.FalseBlock.Accept(this);
            }

            _il.MarkLabel(endLabel);
        }


        private void EmitBranchTrue(Expression expression, ILGeneratorLabel label)
        {
            EmitBranch(true, expression, label);
        }

        private void EmitBranchFalse(Expression expression, ILGeneratorLabel label)
        {
            EmitBranch(false, expression, label);
        }

        private void EmitBranch(bool branchOnTrue, BinaryExpression expression, ILGeneratorLabel label)
        {
            switch (expression.Operator)
            {
                case BinaryOperatorType.TypeTest:
                    EmitTypeTest(expression);
                    _il.Emit(branchOnTrue ? OperationCode.Brtrue : OperationCode.Brfalse, label);
                    break;

                case BinaryOperatorType.Or:
                    if (branchOnTrue)
                    {
                        EmitBranch(true, expression.Left, label);
                        EmitBranch(true, expression.Right, label);
                    }
                    else
                    {
                        ILGeneratorLabel skipRhs = new ILGeneratorLabel();
                        EmitBranch(true, expression.Left, skipRhs);
                        EmitBranch(false, expression.Right, label);
                        _il.MarkLabel(skipRhs);
                    }
                    break;

                case BinaryOperatorType.And:
                    if (branchOnTrue)
                    {
                        ILGeneratorLabel skipRhs = new ILGeneratorLabel();
                        EmitBranch(false, expression.Left, skipRhs);
                        EmitBranch(true, expression.Right, label);
                        _il.MarkLabel(skipRhs);
                    }
                    else
                    {
                        EmitBranch(false, expression.Left, label);
                        EmitBranch(false, expression.Right, label);
                    }
                    break;

                case BinaryOperatorType.Equality:
                    if (IsZeroEquivalent(expression.Left))
                        EmitBranch(!branchOnTrue, expression.Right, label);
                    else if (IsZeroEquivalent(expression.Right))
                        EmitBranch(!branchOnTrue, expression.Left, label);
                    else
                    {
                        LoadCmpOperands(expression);
                        _il.Emit(branchOnTrue ? OperationCode.Beq : OperationCode.Bne_Un, label);
                    }
                    break;

                case BinaryOperatorType.Inequality:
                    if (IsZeroEquivalent(expression.Left))
                    {
                        EmitBranch(branchOnTrue, expression.Right, label);
                    }
                    else if (IsZeroEquivalent(expression.Right))
                    {
                        EmitBranch(branchOnTrue, expression.Left, label);
                    }
                    else
                    {
                        LoadCmpOperands(expression);
                        _il.Emit(branchOnTrue ? OperationCode.Bne_Un : OperationCode.Beq, label);
                    }
                    break;

                case BinaryOperatorType.ReferenceEquality:
                    if (IsNull(expression.Left))
                    {
                        EmitRawBranch(!branchOnTrue, expression.Right, label);
                        break;
                    }
                    if (IsNull(expression.Right))
                    {
                        EmitRawBranch(!branchOnTrue, expression.Left, label);
                        break;
                    }
                    Visit(expression.Left); PopType();
                    Visit(expression.Right); PopType();
                    _il.Emit(branchOnTrue ? OperationCode.Beq : OperationCode.Bne_Un, label);
                    break;

                case BinaryOperatorType.ReferenceInequality:
                    if (IsNull(expression.Left))
                    {
                        EmitRawBranch(branchOnTrue, expression.Right, label);
                        break;
                    }
                    if (IsNull(expression.Right))
                    {
                        EmitRawBranch(branchOnTrue, expression.Left, label);
                        break;
                    }
                    Visit(expression.Left); PopType();
                    Visit(expression.Right); PopType();
                    _il.Emit(branchOnTrue ? OperationCode.Bne_Un : OperationCode.Beq, label);
                    break;

                case BinaryOperatorType.GreaterThan:
                    LoadCmpOperands(expression);
                    _il.Emit(branchOnTrue ? OperationCode.Bgt : OperationCode.Ble, label);
                    break;

                case BinaryOperatorType.GreaterThanOrEqual:
                    LoadCmpOperands(expression);
                    _il.Emit(branchOnTrue ? OperationCode.Bge : OperationCode.Blt, label);
                    break;

                case BinaryOperatorType.LessThan:
                    LoadCmpOperands(expression);
                    _il.Emit(branchOnTrue ? OperationCode.Blt : OperationCode.Bge, label);
                    break;

                case BinaryOperatorType.LessThanOrEqual:
                    LoadCmpOperands(expression);
                    _il.Emit(branchOnTrue ? OperationCode.Ble : OperationCode.Bgt, label);
                    break;

                default:
                    EmitDefaultBranch(branchOnTrue, expression, label);
                    break;
            }
        }

        private void EmitBranch(bool branchOnTrue, UnaryExpression expression, ILGeneratorLabel label)
        {
            if (UnaryOperatorType.LogicalNot == expression.Operator)
            {
                EmitBranch(!branchOnTrue, expression.Operand, label);
            }
            else
            {
                EmitDefaultBranch(branchOnTrue, expression, label);
            }
        }

        private void EmitBranch(bool branchOnTrue, Expression expression, ILGeneratorLabel label)
        {
            switch (expression.NodeType)
            {
                case NodeType.BinaryExpression:
                    {
                        EmitBranch(branchOnTrue, (BinaryExpression)expression, label);
                        break;
                    }

                case NodeType.UnaryExpression:
                    {
                        EmitBranch(branchOnTrue, (UnaryExpression)expression, label);
                        break;
                    }

                default:
                    {
                        EmitDefaultBranch(branchOnTrue, expression, label);
                        break;
                    }
            }
        }

        private void EmitRawBranch(bool branch, Expression condition, ILGeneratorLabel label)
        {
            condition.Accept(this); PopType();
            _il.Emit(branch ? OperationCode.Brtrue : OperationCode.Brfalse, label);
        }

        private void EmitDefaultBranch(bool branch, Expression condition, ILGeneratorLabel label)
        {
            if (branch && IsOneEquivalent(condition))
            {
                _il.Emit(OperationCode.Br, label);
                return;
            }

            if (!branch && IsZeroEquivalent(condition))
            {
                _il.Emit(OperationCode.Br, label);
                return;
            }

            condition.Accept(this);

            var type = PopType();
            if (TypeSystemServices.IsFloatingPointNumber(type))
            {
                EmitDefaultValue(type);
                _il.Emit(branch ? OperationCode.Bne_Un : OperationCode.Beq, label);
                return;
            }

            EmitToBoolIfNeeded(condition);
            _il.Emit(branch ? OperationCode.Brtrue : OperationCode.Brfalse, label);
        }

        private static bool IsZeroEquivalent(Expression expression)
        {
            return (IsNull(expression) || IsZero(expression) || IsFalse(expression));
        }

        private static bool IsOneEquivalent(Expression expression)
        {
            return IsBooleanLiteral(expression, true) || IsNumberLiteral(expression, 1);
        }

        private static bool IsNull(Expression expression)
        {
            return NodeType.NullLiteralExpression == expression.NodeType;
        }

        private static bool IsFalse(Expression expression)
        {
            return IsBooleanLiteral(expression, false);
        }

        private static bool IsBooleanLiteral(Expression expression, bool value)
        {
            return NodeType.BoolLiteralExpression == expression.NodeType
                   && (value == ((BoolLiteralExpression)expression).Value);
        }

        private static bool IsZero(Expression expression)
        {
            return IsNumberLiteral(expression, 0);
        }

        private static bool IsNumberLiteral(Expression expression, int value)
        {
            return (NodeType.IntegerLiteralExpression == expression.NodeType
                    && (value == ((IntegerLiteralExpression)expression).Value))
                   ||
                   (NodeType.DoubleLiteralExpression == expression.NodeType
                    && (value == ((DoubleLiteralExpression)expression).Value));
        }

        public override void OnBreakStatement(BreakStatement node)
        {
            EmitGoTo(_currentLoopInfo.BreakLabel, node);
        }

        private void EmitGoTo(ILGeneratorLabel label, Node debugInfo)
        {
            EmitDebugInfo(debugInfo);
            _il.Emit(InTryInLoop() ? OperationCode.Leave : OperationCode.Br, label);
        }

        public override void OnContinueStatement(ContinueStatement node)
        {
            EmitGoTo(_currentLoopInfo.ContinueLabel, node);
        }

        public override void OnWhileStatement(WhileStatement node)
        {
            ILGeneratorLabel endLabel = new ILGeneratorLabel();
            ILGeneratorLabel bodyLabel = new ILGeneratorLabel();
            ILGeneratorLabel conditionLabel = new ILGeneratorLabel();

            _il.Emit(OperationCode.Br, conditionLabel);
            _il.MarkLabel(bodyLabel);

            EnterLoop(endLabel, conditionLabel);
            node.Block.Accept(this);
            LeaveLoop();

            _il.MarkLabel(conditionLabel);
            EmitDebugInfo(node);
            EmitBranchTrue(node.Condition, bodyLabel);
            Visit(node.OrBlock);
            Visit(node.ThenBlock);
            _il.MarkLabel(endLabel);
        }

        private void EmitIntNot()
        {
            _il.Emit(OperationCode.Ldc_I4_0);
            _il.Emit(OperationCode.Ceq);
        }

        private void EmitGenericNot()
        {
            // bool codification:
            // value_on_stack ? 0 : 1
            ILGeneratorLabel wasTrue = new ILGeneratorLabel();
            ILGeneratorLabel wasFalse = new ILGeneratorLabel();
            _il.Emit(OperationCode.Brfalse_S, wasFalse);
            _il.Emit(OperationCode.Ldc_I4_0);
            _il.Emit(OperationCode.Br_S, wasTrue);
            _il.MarkLabel(wasFalse);
            _il.Emit(OperationCode.Ldc_I4_1);
            _il.MarkLabel(wasTrue);
        }

        public override void OnUnaryExpression(UnaryExpression node)
        {
            switch (node.Operator)
            {
                case UnaryOperatorType.LogicalNot:
                    {
                        EmitLogicalNot(node);
                        break;
                    }

                case UnaryOperatorType.UnaryNegation:
                    {
                        EmitUnaryNegation(node);
                        break;
                    }

                case UnaryOperatorType.OnesComplement:
                    {
                        EmitOnesComplement(node);
                        break;
                    }

                case UnaryOperatorType.AddressOf:
                    {
                        EmitAddressOf(node);
                        break;
                    }

                case UnaryOperatorType.Indirection:
                    {
                        EmitIndirection(node);
                        break;
                    }

                default:
                    {
                        NotImplemented(node, "unary operator not supported");
                        break;
                    }
            }
        }

        IType _byAddress = null;
        bool IsByAddress(IType type)
        {
            return (_byAddress == type);
        }

        private void EmitDefaultValue(IType type)
        {
            var isGenericParameter = GenericsServices.IsGenericParameter(type);

            if (!type.IsValueType && !isGenericParameter)
                _il.Emit(OperationCode.Ldnull);
            else if (type == TypeSystemServices.BoolType)
                _il.Emit(OperationCode.Ldc_I4_0);
            else if (TypeSystemServices.IsFloatingPointNumber(type))
                EmitLoadLiteral(type, 0.0);
            else if (TypeSystemServices.IsPrimitiveNumber(type) || type == TypeSystemServices.CharType)
                EmitLoadLiteral(type, 0);
            else if (isGenericParameter && TypeSystemServices.IsReferenceType(type))
            {
                _il.Emit(OperationCode.Ldnull);
                _il.Emit(OperationCode.Unbox_Any, GetSystemType(type));
            }
            else //valuetype or valuetype/unconstrained generic parameter
            {
                //TODO: if MethodBody.InitLocals is false
                //_il.Emit(OperationCode.Ldloca, GetDefaultValueHolder(type));
                //_il.Emit(OperationCode.Initobj, GetSystemType(type));
                _il.Emit(OperationCode.Ldloc, GetDefaultValueHolder(type));
            }

            PushType(type);
        }

	    readonly Dictionary<IType, LocalDefinition> _defaultValueHolders = new Dictionary<IType, LocalDefinition>();

        //get the default value holder (a local actually) for a given type
        //default value holder pool is cleared before each method body processing
        LocalDefinition GetDefaultValueHolder(IType type)
        {
            LocalDefinition holder;
            if (_defaultValueHolders.TryGetValue(type, out holder))
                return holder;

            holder = DefineLocal(null, GetSystemType(type));
            _defaultValueHolders.Add(type, holder);
            return holder;
        }


        private void EmitOnesComplement(UnaryExpression node)
        {
            node.Operand.Accept(this);
            _il.Emit(OperationCode.Not);
        }

        private void EmitLogicalNot(UnaryExpression node)
        {
            Expression operand = node.Operand;
            operand.Accept(this);
            IType typeOnStack = PopType();
            bool notContext = true;

            if (IsBoolOrInt(typeOnStack))
            {
                EmitIntNot();
            }
            else if (EmitToBoolIfNeeded(operand, ref notContext))
            {
                if (!notContext) //we are in a not context and emit to bool is also in a not context
                    EmitIntNot();//so we do not need any not (false && false => true)
            }
            else
            {
                EmitGenericNot();
            }
            PushBool();
        }

        private void EmitUnaryNegation(UnaryExpression node)
        {
            var operandType = GetExpressionType(node.Operand);
            if (IsCheckedIntegerOperand(operandType))
            {
                _il.Emit(OperationCode.Ldc_I4_0);
                if (IsLong(operandType) || operandType == TypeSystemServices.ULongType)
                    _il.Emit(OperationCode.Conv_I8);
                node.Operand.Accept(this);
                _il.Emit(TypeSystemServices.IsSignedNumber(operandType)
                             ? OperationCode.Sub_Ovf
                             : OperationCode.Sub_Ovf_Un);
                if (!IsLong(operandType) && operandType != TypeSystemServices.ULongType)
                    EmitCastIfNeeded(operandType, TypeSystemServices.IntType);
            }
            else
            {
                //a single/double unary negation never overflow
                node.Operand.Accept(this);
                _il.Emit(OperationCode.Neg);
            }
        }

        private bool IsCheckedIntegerOperand(IType operandType)
        {
            return _checked && IsInteger(operandType);
        }

        private void EmitAddressOf(UnaryExpression node)
        {
            _byAddress = GetExpressionType(node.Operand);
            node.Operand.Accept(this);
            PushType(PopType().MakePointerType());
            _byAddress = null;
        }

        private void EmitIndirection(UnaryExpression node)
        {
            node.Operand.Accept(this);

            if (node.Operand.NodeType != NodeType.ReferenceExpression
                && node.ParentNode.NodeType != NodeType.MemberReferenceExpression)
            {
                //pointer arithmetic, need to load the address
                IType et = PeekTypeOnStack().ElementType;
                OperationCode code = GetLoadRefParamCode(et);
                if (code == OperationCode.Ldobj)
                    _il.Emit(code, GetSystemType(et));
                else
                    _il.Emit(code);

                PopType();
                PushType(et);
            }
        }

        static bool ShouldLeaveValueOnStack(Expression node)
        {
            return node.ParentNode.NodeType != NodeType.ExpressionStatement;
        }

        private void OnReferenceComparison(BinaryExpression node)
        {
            node.Left.Accept(this); PopType();
            node.Right.Accept(this); PopType();
            _il.Emit(OperationCode.Ceq);
            if (BinaryOperatorType.ReferenceInequality == node.Operator)
            {
                EmitIntNot();
            }
            PushBool();
        }

        private void OnAssignmentToSlice(BinaryExpression node)
        {
            var slice = (SlicingExpression)node.Left;
            Visit(slice.Target);

            var arrayType = (TypeSystem.IArrayType)PopType();
            if (arrayType.Rank == 1)
                EmitAssignmentToSingleDimensionalArrayElement(arrayType, slice, node);
            else
                EmitAssignmentToMultiDimensionalArrayElement(arrayType, slice, node);
        }

        private void EmitAssignmentToMultiDimensionalArrayElement(TypeSystem.IArrayType arrayType, SlicingExpression slice, BinaryExpression node)
        {
            var elementType = arrayType.ElementType;
            LoadArrayIndices(slice);
            var temp = LoadAssignmentOperand(elementType, node);
            CallArraySet(arrayType);
            FlushAssignmentOperand(elementType, temp);
        }

        private void EmitAssignmentToSingleDimensionalArrayElement(TypeSystem.IArrayType arrayType, SlicingExpression slice, BinaryExpression node)
        {
            var elementType = arrayType.ElementType;

            var index = slice.Indices[0];
            EmitNormalizedArrayIndex(slice, index.Begin);

            var opcode = GetStoreEntityOpCode(elementType);
            bool stobj = IsStobj(opcode);
            if (stobj) _il.Emit(OperationCode.Ldelema, GetSystemType(elementType));

            var temp = LoadAssignmentOperand(elementType, node);

            if (stobj)
                _il.Emit(opcode, GetSystemType(elementType));
            else
                _il.Emit(opcode);

            FlushAssignmentOperand(elementType, temp);
        }

        private void FlushAssignmentOperand(IType elementType, LocalDefinition temp)
        {
            if (temp != null)
                LoadLocal(temp, elementType);
            else
                PushVoid();
        }

        private LocalDefinition LoadAssignmentOperand(IType elementType, BinaryExpression node)
        {
            LoadExpressionWithType(elementType, node.Right);
            var leaveValueOnStack = ShouldLeaveValueOnStack(node);
            LocalDefinition temp = null;
            if (leaveValueOnStack)
            {
                Dup();
                temp = StoreTempLocal(elementType);
            }
            return temp;
        }

        LocalDefinition _currentLocal = null;

        private void LoadLocal(LocalDefinition local, IType localType)
        {
            _il.Emit(OperationCode.Ldloc, local);

            PushType(localType);
            _currentLocal = local;
        }

        private void LoadLocal(InternalLocal local)
        {
            LoadLocal(local, false);
        }

        private void LoadLocal(InternalLocal local, bool byAddress)
        {
            _il.Emit(IsByAddress(local.Type) ? OperationCode.Ldloca : OperationCode.Ldloc, local.LocalDefinition);

            PushType(local.Type);
            _currentLocal = local.LocalDefinition;
        }

        private void LoadIndirectLocal(InternalLocal local)
        {
            LoadLocal(local);

            IType et = local.Type.ElementType;
            PopType();
            PushType(et);
            OperationCode code = GetLoadRefParamCode(et);
            if (code == OperationCode.Ldobj)
                _il.Emit(code, GetSystemType(et));
            else
                _il.Emit(code);
        }

        private LocalDefinition StoreTempLocal(IType elementType)
        {
            LocalDefinition temp = DefineLocal(null, GetSystemType(elementType) );
            _il.Emit(OperationCode.Stloc, temp);
            return temp;
        }

        private void OnAssignment(BinaryExpression node)
        {
            if (NodeType.SlicingExpression == node.Left.NodeType)
            {
                OnAssignmentToSlice(node);
                return;
            }

            // when the parent is not a statement we need to leave
            // the value on the stack
            bool leaveValueOnStack = ShouldLeaveValueOnStack(node);
            IEntity tag = TypeSystemServices.GetEntity(node.Left);
            switch (tag.EntityType)
            {
                case EntityType.Local:
                    {
                        SetLocal(node, (InternalLocal)tag, leaveValueOnStack);
                        break;
                    }

                case EntityType.Parameter:
                    {
                        InternalParameter param = (InternalParameter)tag;
                        if (param.Parameter.IsByRef)
                        {
                            SetByRefParam(param, node.Right, leaveValueOnStack);
                            break;
                        }

                        LoadExpressionWithType(param.Type, node.Right);

                        if (leaveValueOnStack)
                        {
                            Dup();
                            PushType(param.Type);
                        }
                        _il.Emit(OperationCode.Starg, (ParameterDefinition) GetBuilder(param.Parameter));
                        break;
                    }

                case EntityType.Field:
                    {
                        IField field = (IField)tag;
                        SetField(node, field, node.Left, node.Right, leaveValueOnStack);
                        break;
                    }

                case EntityType.Property:
                    {
                        SetProperty((IProperty)tag, node.Left, node.Right, leaveValueOnStack);
                        break;
                    }

                case EntityType.Event: //event=null (always internal in this context)
                    {
                        InternalEvent e = (InternalEvent)tag;
                        OperationCode opcode = e.IsStatic ? OperationCode.Stsfld : OperationCode.Stfld;
                        _il.Emit(OperationCode.Ldnull);
                        _il.Emit(opcode, GetFieldBuilder(e.BackingField.Field));
                        break;
                    }

                default:
                    {
                        NotImplemented(node, tag.ToString());
                        break;
                    }
            }
            if (!leaveValueOnStack)
            {
                PushVoid();
            }
        }

        private void SetByRefParam(InternalParameter param, Expression right, bool leaveValueOnStack)
        {
            LocalDefinition temp = null;
            IType tempType = null;
            if (leaveValueOnStack)
            {
                Visit(right);
                tempType = PopType();
                temp = StoreTempLocal(tempType);
            }

            LoadParam(param);
            if (temp != null)
            {
                LoadLocal(temp, tempType);
                PopType();
            }
            else
                LoadExpressionWithType(param.Type, right);

            var storecode = GetStoreRefParamCode(param.Type);
            if (IsStobj(storecode)) //passing struct/decimal byref
                _il.Emit(storecode, GetSystemType(param.Type));
            else
                _il.Emit(storecode);

            if (null != temp)
                LoadLocal(temp, tempType);
        }

        private void EmitTypeTest(BinaryExpression node)
        {
            Visit(node.Left);
            IType actualType = PopType();

            EmitBoxIfNeeded(TypeSystemServices.ObjectType, actualType);

            var type = NodeType.TypeofExpression == node.Right.NodeType
                ? GetSystemType(((TypeofExpression)node.Right).Type)
                : GetSystemType(node.Right);

            _il.Emit(OperationCode.Isinst, type);
        }

        private void OnTypeTest(BinaryExpression node)
        {
            EmitTypeTest(node);

            _il.Emit(OperationCode.Ldnull);
            _il.Emit(OperationCode.Cgt_Un);

            PushBool();
        }

        private void LoadCmpOperands(BinaryExpression node)
        {
            var lhs = node.Left.ExpressionType;
            var rhs = node.Right.ExpressionType;
            if (lhs != rhs)
            {
                var type = TypeSystemServices.GetPromotedNumberType(lhs, rhs);
                LoadExpressionWithType(type, node.Left);
                LoadExpressionWithType(type, node.Right);
            }
            else //no need for conversion
            {
                Visit(node.Left);
                PopType();
                Visit(node.Right);
                PopType();
            }
        }

        private void OnEquality(BinaryExpression node)
        {
            LoadCmpOperands(node);
            _il.Emit(OperationCode.Ceq);
            PushBool();
        }

        private void OnInequality(BinaryExpression node)
        {
            LoadCmpOperands(node);
            _il.Emit(OperationCode.Ceq);
            EmitIntNot();
            PushBool();
        }

        private void OnGreaterThan(BinaryExpression node)
        {
            LoadCmpOperands(node);
            _il.Emit(OperationCode.Cgt);
            PushBool();
        }

        private void OnGreaterThanOrEqual(BinaryExpression node)
        {
            OnLessThan(node);
            EmitIntNot();
        }

        private void OnLessThan(BinaryExpression node)
        {
            LoadCmpOperands(node);
            _il.Emit(OperationCode.Clt);
            PushBool();
        }

        private void OnLessThanOrEqual(BinaryExpression node)
        {
            OnGreaterThan(node);
            EmitIntNot();
        }

        private void OnExponentiation(BinaryExpression node)
        {
            var doubleType = TypeSystemServices.DoubleType;
            LoadOperandsWithType(doubleType, node);
            Call(_mathPow);
            PushType(doubleType);
        }

        private void LoadOperandsWithType(IType type, BinaryExpression node)
        {
            LoadExpressionWithType(type, node.Left);
            LoadExpressionWithType(type, node.Right);
        }

        private void OnArithmeticOperator(BinaryExpression node)
        {
            var type = node.ExpressionType;
            LoadOperandsWithType(type, node);
            _il.Emit(GetArithmeticOpCode(type, node.Operator));
            PushType(type);
        }

        bool EmitToBoolIfNeeded(Expression expression)
        {
            bool notContext = false;
            return EmitToBoolIfNeeded(expression, ref notContext);
        }

        //mostly used for logical operators and ducky mode
        //other cases of bool context are handled by PMB
        bool EmitToBoolIfNeeded(Expression expression, ref bool notContext)
        {
            bool inNotContext = notContext;
            notContext = false;

            //use a builtin conversion operator just for the logical operator trueness test
            IType type = GetExpressionType(expression);
            if (TypeSystemServices.ObjectType == type || TypeSystemServices.DuckType == type)
            {
                Call(_runtimeServicesToBoolObject);
                return true;
            }
            else if (TypeSystemServices.IsNullable(type))
            {
                _il.Emit(OperationCode.Ldloca, _currentLocal);
                var sType = GetSystemType(TypeSystemServices.GetNullableUnderlyingType(type));
                Call(GetNullableHasValue(sType));
                LocalDefinition hasValue = StoreTempLocal(TypeSystemServices.BoolType);
                _il.Emit(OperationCode.Pop); //pop nullable address (ldloca)
                _il.Emit(OperationCode.Ldloc, hasValue);
                return true;
            }
            else if (TypeSystemServices.StringType == type)
            {
                Call(_stringIsNullOrEmpty);
                if (!inNotContext)
                    EmitIntNot(); //reverse result (true for not empty)
                else
                    notContext = true;
                return true;
            }
            else if (IsInteger(type))
            {
                if (IsLong(type) || TypeSystemServices.ULongType == type)
                    _il.Emit(OperationCode.Conv_I4);
                return true;
            }
            else if (TypeSystemServices.SingleType == type)
            {
                EmitDefaultValue(TypeSystemServices.SingleType);
                _il.Emit(OperationCode.Ceq);
                if (!inNotContext)
                    EmitIntNot();
                else
                    notContext = true;
                return true;
            }
            else if (TypeSystemServices.DoubleType == type)
            {
                EmitDefaultValue(TypeSystemServices.DoubleType);
                _il.Emit(OperationCode.Ceq);
                if (!inNotContext)
                    EmitIntNot();
                else
                    notContext = true;
                return true;
            }
            else if (TypeSystemServices.DecimalType == type)
            {
                Call(_runtimeServicesToBoolDecimal);
                return true;
            }
            else if (!type.IsValueType)
            {
                if (null == expression.GetAncestor<BinaryExpression>()
                    && null != expression.GetAncestor<IfStatement>())
                    return true; //use br(true|false) directly (most common case)

                _il.Emit(OperationCode.Ldnull);
                if (!inNotContext)
                {
                    _il.Emit(OperationCode.Cgt_Un);
                }
                else
                {
                    _il.Emit(OperationCode.Ceq);
                    notContext = true;
                }
                return true;
            }
            return false;
        }

        private void EmitAnd(BinaryExpression node)
        {
            EmitLogicalOperator(node,  OperationCode.Brtrue, OperationCode.Brfalse);
        }

        private void EmitOr(BinaryExpression node)
        {
            EmitLogicalOperator(node, OperationCode.Brfalse, OperationCode.Brtrue);
        }

        private void EmitLogicalOperator(BinaryExpression node, OperationCode brForValueType, OperationCode brForRefType)
        {
            var type = GetExpressionType(node);
            Visit(node.Left);

            var lhsType = PopType();

            if (lhsType != null && lhsType.IsValueType && !type.IsValueType)
            {
                // if boxing, first evaluate the value
                // as it is and then box it...
                ILGeneratorLabel evalRhs = new ILGeneratorLabel();
                ILGeneratorLabel end = new ILGeneratorLabel();

                Dup();
                EmitToBoolIfNeeded(node.Left);  // may need to convert decimal to bool
                _il.Emit(brForValueType, evalRhs);
                EmitCastIfNeeded(type, lhsType);
                _il.Emit(OperationCode.Br_S, end);

                _il.MarkLabel(evalRhs);
                _il.Emit(OperationCode.Pop);
                LoadExpressionWithType(type, node.Right);

                _il.MarkLabel(end);
            }
            else
            {
                ILGeneratorLabel end = new ILGeneratorLabel();

                EmitCastIfNeeded(type, lhsType);
                Dup();

                EmitToBoolIfNeeded(node.Left);

                _il.Emit(brForRefType, end);

                _il.Emit(OperationCode.Pop);
                LoadExpressionWithType(type, node.Right);
                _il.MarkLabel(end);
            }

            PushType(type);
        }

        IType GetExpectedTypeForBitwiseRightOperand(BinaryExpression node)
        {
            switch (node.Operator)
            {
                case BinaryOperatorType.ShiftLeft:
                case BinaryOperatorType.ShiftRight:
                    return TypeSystemServices.IntType;
            }
            return GetExpressionType(node);
        }

        private void EmitBitwiseOperator(BinaryExpression node)
        {
            var type = node.ExpressionType;
            LoadExpressionWithType(type, node.Left);
            LoadExpressionWithType(GetExpectedTypeForBitwiseRightOperand(node), node.Right);

            switch (node.Operator)
            {
                case BinaryOperatorType.BitwiseOr:
                    {
                        _il.Emit(OperationCode.Or);
                        break;
                    }

                case BinaryOperatorType.BitwiseAnd:
                    {
                        _il.Emit(OperationCode.And);
                        break;
                    }

                case BinaryOperatorType.ExclusiveOr:
                    {
                        _il.Emit(OperationCode.Xor);
                        break;
                    }

                case BinaryOperatorType.ShiftLeft:
                    {
                        _il.Emit(OperationCode.Shl);
                        break;
                    }
                case BinaryOperatorType.ShiftRight:
                    {
                        _il.Emit(TypeSystemServices.IsSignedNumber(type) ? OperationCode.Shr : OperationCode.Shr_Un);
                        break;
                    }
            }

            PushType(type);
        }

        public override void OnBinaryExpression(BinaryExpression node)
        {
            switch (node.Operator)
            {
                case BinaryOperatorType.ShiftLeft:
                case BinaryOperatorType.ShiftRight:
                case BinaryOperatorType.ExclusiveOr:
                case BinaryOperatorType.BitwiseAnd:
                case BinaryOperatorType.BitwiseOr:
                    {
                        EmitBitwiseOperator(node);
                        break;
                    }

                case BinaryOperatorType.Or:
                    {
                        EmitOr(node);
                        break;
                    }

                case BinaryOperatorType.And:
                    {
                        EmitAnd(node);
                        break;
                    }

                case BinaryOperatorType.Addition:
                case BinaryOperatorType.Subtraction:
                case BinaryOperatorType.Multiply:
                case BinaryOperatorType.Division:
                case BinaryOperatorType.Modulus:
                    {
                        OnArithmeticOperator(node);
                        break;
                    }

                case BinaryOperatorType.Exponentiation:
                    {
                        OnExponentiation(node);
                        break;
                    }

                case BinaryOperatorType.Assign:
                    {
                        OnAssignment(node);
                        break;
                    }

                case BinaryOperatorType.Equality:
                    {
                        OnEquality(node);
                        break;
                    }

                case BinaryOperatorType.Inequality:
                    {
                        OnInequality(node);
                        break;
                    }

                case BinaryOperatorType.GreaterThan:
                    {
                        OnGreaterThan(node);
                        break;
                    }

                case BinaryOperatorType.LessThan:
                    {
                        OnLessThan(node);
                        break;
                    }

                case BinaryOperatorType.GreaterThanOrEqual:
                    {
                        OnGreaterThanOrEqual(node);
                        break;
                    }

                case BinaryOperatorType.LessThanOrEqual:
                    {
                        OnLessThanOrEqual(node);
                        break;
                    }

                case BinaryOperatorType.ReferenceInequality:
                    {
                        OnReferenceComparison(node);
                        break;
                    }

                case BinaryOperatorType.ReferenceEquality:
                    {
                        OnReferenceComparison(node);
                        break;
                    }

                case BinaryOperatorType.TypeTest:
                    {
                        OnTypeTest(node);
                        break;
                    }

                default:
                    {
                        OperatorNotImplemented(node);
                        break;
                    }
            }
        }

        private void OperatorNotImplemented(BinaryExpression node)
        {
            NotImplemented(node, node.Operator.ToString());
        }

        public override void OnTypeofExpression(TypeofExpression node)
        {
            EmitGetTypeFromHandle(GetSystemType(node.Type));
        }

        public override void OnCastExpression(CastExpression node)
        {
            var type = GetType(node.Type);
            LoadExpressionWithType(type, node.Target);
            PushType(type);
        }

        public override void OnTryCastExpression(TryCastExpression node)
        {
            var type = GetSystemType(node.Type);
            node.Target.Accept(this); PopType();
            Isinst(type);
            PushType(node.ExpressionType);
        }

        private void Isinst(ITypeReference type)
        {
            _il.Emit(OperationCode.Isinst, type);
        }

        private void InvokeMethod(IMethod method, MethodInvocationExpression node)
        {
            var mi = GetMethodInfo(method);
            if (!InvokeOptimizedMethod(method, mi, node))
                InvokeRegularMethod(method, mi, node);
        }

	    private int _matrixNameKey;
	    private IMethodReference _arrayGetLength;
	    private ITypeDefinition _builtinsType;
	    private INamedTypeDefinition _nullableType;
	    private IMethodReference _builtinsArrayGenericConstructor;
	    private IMethodReference _builtinsArrayTypedConstructor;
	    private IMethodReference _builtinsArrayTypedCollectionConstructor;
        private IMethodReference _builtinsTypedMatrixConstructor;
        private IMethodReference _timeSpanLongConstructor;
        private IMethodReference _hashConstructor;
        private IMethodReference _listEmptyConstructor;
        private IMethodReference _listArrayBoolConstructor;
	    private IMethodReference _regexConstructor;
        private IMethodReference _regexConstructorOptions;
	    private IMethodReference _stringBuilderToString;
        private IMethodReference _stringBuilderConstructor;
        private IMethodReference _stringBuilderConstructorString;
        private IMethodReference _stringBuilderAppendObject;
        private IMethodReference _stringBuilderAppendString;
        private IMethodReference _mathPow;
	    private IMethodReference _runtimeServicesToBoolObject;
	    private IMethodReference _runtimeServicesToBoolDecimal;
	    private IMethodReference _runtimeServicesNormalizeArrayIndex;
	    private IMethodReference _stringIsNullOrEmpty;
	    private IMethodReference _typeGetTypeFromHandle;
	    private IMethodReference _hashAdd;
	    private IMethodReference _runtimeHelpersInitializeArray;
	    private IFieldReference _stringEmpty;

        private void SetupBuiltins()
        {
            _matrixNameKey = _nameTable.GetNameFor("matrix").UniqueKey;
            _arrayGetLength = PropertyOf<Array>("Length").Getter;
            _builtinsTypedMatrixConstructor = MethodOf<Array>("CreateInstance", typeof(Type), typeof(int[]));
            _builtinsType = GetTypeReference<Builtins>();
            _nullableType = (INamedTypeDefinition)GetTypeReference(typeof(Nullable<>));
            _builtinsArrayGenericConstructor = MethodOf<Builtins>("array", typeof(int));
            _builtinsArrayTypedConstructor = MethodOf<Builtins>("array", typeof(Type), typeof(int));
            _builtinsArrayTypedCollectionConstructor = MethodOf<Builtins>("array", typeof(Type), typeof(ICollection));
            _timeSpanLongConstructor = ConstructorOf<TimeSpan>(typeof(long));
            _hashConstructor = ConstructorOf<Hash>();
            _listEmptyConstructor = ConstructorOf<List>();
            _listArrayBoolConstructor = ConstructorOf<List>(typeof(object[]), typeof(bool));
            _regexConstructor = ConstructorOf<Regex>(typeof(string));
            _regexConstructorOptions = ConstructorOf<Regex>(typeof(string), typeof(RegexOptions));
            _stringBuilderToString = MethodOf<StringBuilder>("ToString");
            _stringBuilderConstructor = ConstructorOf<StringBuilder>();
            _stringBuilderConstructorString = ConstructorOf<StringBuilder>(typeof(string));
            _stringBuilderAppendObject = MethodOf<StringBuilder>("Append", typeof(object));
            _stringBuilderAppendString = MethodOf<StringBuilder>("Append", typeof(string));
            _mathPow = MethodOf(GetTypeReference(typeof(Math)), "Pow", typeof(double), typeof(double));
            _runtimeServicesToBoolObject = MethodOf<RuntimeServices>("ToBool", typeof(object));
            _runtimeServicesToBoolDecimal = MethodOf<RuntimeServices>("ToBool", typeof(decimal));
            _stringIsNullOrEmpty = MethodOf<string>("IsNullOrEmpty", typeof(string));
            _typeGetTypeFromHandle = MethodOf<Type>("GetTypeFromHandle", typeof(RuntimeTypeHandle));
            _hashAdd = MethodOf<Hashtable>("Add", typeof(object), typeof(object));
            _runtimeServicesNormalizeArrayIndex = MethodOf<RuntimeServices>("NormalizeArrayIndex", typeof(Array), typeof(int));
            _runtimeHelpersInitializeArray =
                MethodOf(GetTypeReference(typeof(System.Runtime.CompilerServices.RuntimeHelpers)), "InitializeArray", typeof(Array), typeof(RuntimeFieldHandle));
            _stringEmpty = TypeHelper.GetField(GetTypeReference<string>(), _nameTable.GetNameFor("Empty"));
        }

	    private bool MethodsEqual(IMethodReference m1, IMethodReference m2)
	    {
	        return _host.InternFactory.GetMethodInternedKey(m1) == _host.InternFactory.GetMethodInternedKey(m2);
	    }

        private bool TypesEqual(ITypeReference t1, ITypeReference t2)
        {
            return _host.InternFactory.GetTypeReferenceInternedKey(t1) == _host.InternFactory.GetTypeReferenceInternedKey(t2);
        }

        private bool InvokeOptimizedMethod(IMethod method, IMethodDefinition mi, MethodInvocationExpression node)
        {
            if (MethodsEqual(_arrayGetLength, mi))
            {
                // don't use ldlen for System.Array
                if (!GetType(node.Target).IsArray)
                    return false;

                // optimize constructs such as:
                //		len(anArray)
                //		anArray.Length
                Visit(node.Target);
                PopType();
                _il.Emit(OperationCode.Ldlen);
                PushType(TypeSystemServices.IntType);
                return true;
            }

            if (!TypesEqual(mi.ContainingTypeDefinition, _builtinsType))
                return false;

            var gen = mi as IGenericMethodInstance;
            if (gen != null)
            {
                if (MethodsEqual(_builtinsArrayGenericConstructor, gen.GenericMethod))
                {
                    // optimize constructs such as:
                    //		array[of int](2)
                    IType type = method.ConstructedInfo.GenericArguments[0];
                    EmitNewArray(type, node.Arguments[0]);
                    return true;
                }

                if (mi.Name.UniqueKey == _matrixNameKey)
                {
                    EmitNewMatrix(node);
                    return true;
                }
                return false;
            }

            if (MethodsEqual(mi, _builtinsArrayTypedConstructor))
            {
                // optimize constructs such as:
                //		array(int, 2)
                IType type = TypeSystemServices.GetReferencedType(node.Arguments[0]);
                if (null != type)
                {
                    EmitNewArray(type, node.Arguments[1]);
                    return true;
                }
            }
            else if (MethodsEqual(mi, _builtinsArrayTypedCollectionConstructor))
            {
                // optimize constructs such as:
                //		array(int, (1, 2, 3))
                //		array(byte, [1, 2, 3, 4])
                IType type = TypeSystemServices.GetReferencedType(node.Arguments[0]);
                if (null != type)
                {
                    ListLiteralExpression items = node.Arguments[1] as ListLiteralExpression;
                    if (null != items)
                    {
                        EmitArray(type, items.Items);
                        PushType(type.MakeArrayType(1));
                        return true;
                    }
                }
            }
            return false;
        }

        private void EmitNewMatrix(MethodInvocationExpression node)
        {
            var expressionType = GetExpressionType(node);
            var matrixType = (IArrayTypeReference)GetSystemType(expressionType);

            // matrix of type(dimensions)
            EmitGetTypeFromHandle(matrixType.ElementType);
            PopType();

            EmitArray(TypeSystemServices.IntType, node.Arguments);

            Call(Array_CreateInstance);
            Castclass(matrixType);
            PushType(expressionType);
        }

        private IMethodDefinition Array_CreateInstance
        {
            get
            {
                return _builtinsTypedMatrixConstructor.ResolvedMethod;
            }
        }

        private void EmitNewArray(IType type, Expression length)
        {
            LoadIntExpression(length);
            _il.Emit(OperationCode.Newarr, new VectorTypeReference{ElementType = GetSystemType(type)});
            PushType(type.MakeArrayType(1));
        }

        private void InvokeRegularMethod(IMethod method, IMethodDefinition mi, MethodInvocationExpression node)
        {
            // Do not emit call if conditional attributes (if any) do not match defined symbols
            if (!CheckConditionalAttributes(method))
            {
                EmitNop();
                PushType(method.ReturnType); // keep a valid state
                return;
            }

            IType targetType = null;
            Expression target = null;
            if (!mi.IsStatic)
            {
                target = GetTargetObject(node);
                targetType = target.ExpressionType;
                PushTargetObjectFor(mi, target, targetType);
            }

            PushArguments(method, node.Arguments);

            // Emit a constrained call if target is a generic parameter
            if (targetType is TypeSystem.IGenericParameter)
                _il.Emit(OperationCode.Constrained_, GetSystemType(targetType));

            mi = ResolveGenericMethod(mi);
            _il.Emit(GetCallOpCode(target, method), mi);

            PushType(method.ReturnType);
        }

        private IMethodDefinition ResolveGenericMethod(IMethodDefinition mi)
        {
            var ctd = mi.ContainingTypeDefinition;
            if (ctd.IsGeneric)
            {
                var baseType = GenericTypeInstance.GetGenericTypeInstance(
                    (INamedTypeDefinition)ctd,
                    ctd.GenericParameters,
                    _host.InternFactory);
                return GetGenericMethod(baseType, mi).ResolvedMethod;
            }
            if (IsNestedGeneric(ctd))
            {
                var baseType = GetSelfMappedClass(ctd);
                mi = TypeHelper.GetMethod(baseType, mi, true);
            }
            return mi;
        }

        //returns true if no conditional attribute match the defined symbols
        //else return false (which means the method won't get emitted)
        private bool CheckConditionalAttributes(IMethod method)
        {
            foreach (string conditionalSymbol in GetConditionalSymbols(method))
                if (!Parameters.Defines.ContainsKey(conditionalSymbol))
                {
                    Context.TraceInfo("call to method '{0}' not emitted because the symbol '{1}' is not defined.", method, conditionalSymbol);
                    return false;
                }
            return true;
        }

        private IEnumerable<string> GetConditionalSymbols(IMethod method)
        {
            var mappedMethod = method as GenericMappedMethod;
            if (mappedMethod != null)
                return GetConditionalSymbols(mappedMethod.SourceMember);

            var constructedMethod = method as GenericConstructedMethod;
            if (constructedMethod != null)
                return GetConditionalSymbols(constructedMethod.GenericDefinition);

            var externalMethod = method as ExternalMethod;
            if (externalMethod != null)
                return GetConditionalSymbols(externalMethod);

            var internalMethod = method as InternalMethod;
            if (internalMethod != null)
                return GetConditionalSymbols(internalMethod);

            return NoSymbols;
        }

        private static readonly string[] NoSymbols = new string[0];

        private static IEnumerable<string> GetConditionalSymbols(ExternalMethod method)
        {
            return from ConditionalAttribute attr in method.MethodInfo.GetCustomAttributes(typeof(ConditionalAttribute), false) select attr.ConditionString;
        }

        private IEnumerable<string> GetConditionalSymbols(InternalMethod method)
        {
            return (from attr in MetadataUtil.GetCustomAttributes(method.Method, TypeSystemServices.ConditionalAttribute)
                where 1 == attr.Arguments.Count select attr.Arguments[0])
                .OfType<StringLiteralExpression>()
                .Select(conditionString => conditionString.Value);
        }

        private void PushTargetObjectFor(IMethodDefinition methodToBeInvoked, Expression target, IType targetType)
        {
            if (targetType is TypeSystem.IGenericParameter)
            {
                // If target is a generic parameter, its address must be loaded
                // to allow a constrained method call
                LoadAddress(target);
                return;
            }

            if (targetType.IsValueType)
            {
                if (methodToBeInvoked.ContainingTypeDefinition.IsValueType)
                    LoadAddress(target);
                else
                {
                    Visit(target);
                    EmitBox(PopType());
                }
                return;
            }

            // pushes target reference
            Visit(target);
            PopType();
        }

        private static Expression GetTargetObject(MethodInvocationExpression node)
        {
            var target = node.Target;

            // Skip over generic reference expressions
            var genericRef = target as GenericReferenceExpression;
            if (genericRef != null)
                target = genericRef.Target;

            var memberRef = target as MemberReferenceExpression;
            if (memberRef != null)
                return memberRef.Target;

            return null;
        }

        private OperationCode GetCallOpCode(Expression target, IMethod method)
        {
            if (method.IsStatic) return OperationCode.Call;
            if (NodeType.SuperLiteralExpression == target.NodeType) return OperationCode.Call;
            if (IsValueTypeMethodCall(target, method)) return OperationCode.Call;
            return OperationCode.Callvirt;
        }

        private bool IsValueTypeMethodCall(Expression target, IMethod method)
        {
            IType type = target.ExpressionType;
            return type.IsValueType && method.DeclaringType == type;
        }

        private void InvokeSuperMethod(IMethod method, MethodInvocationExpression node)
        {
            var super = (IMethod)GetEntity(node.Target);
            var superMI = GetMethodInfo(super);
            if (method.DeclaringType.IsValueType)
                _il.Emit(OperationCode.Ldarga_S, 0);
            else
                _il.Emit(OperationCode.Ldarg_0); // this
            PushArguments(super, node.Arguments);
            Call(superMI);
            PushType(super.ReturnType);
        }

        private void EmitGetTypeFromHandle(ITypeReference type)
        {
            _il.Emit(OperationCode.Ldtoken, type);
            Call(_typeGetTypeFromHandle);
            PushType(TypeSystemServices.TypeType);
        }

        private void OnEval(MethodInvocationExpression node)
        {
            int allButLast = node.Arguments.Count - 1;
            for (int i = 0; i < allButLast; ++i)
            {
                Visit(node.Arguments[i]);
                DiscardValueOnStack();
            }

            Visit(node.Arguments[-1]);
        }

        private void OnAddressOf(MethodInvocationExpression node)
        {
            MemberReferenceExpression methodRef = (MemberReferenceExpression)node.Arguments[0];
            IMethodDefinition method = GetMethodInfo((IMethod)GetEntity(methodRef));
            if (method.IsVirtual)
            {
                Dup();
                _il.Emit(OperationCode.Ldvirtftn, method);
            }
            else
            {
                _il.Emit(OperationCode.Ldftn, method);
            }
            PushType(TypeSystemServices.IntPtrType);
        }

        private void OnBuiltinFunction(BuiltinFunction function, MethodInvocationExpression node)
        {
            switch (function.FunctionType)
            {
                case BuiltinFunctionType.Switch:
                    {
                        OnSwitch(node);
                        break;
                    }

                case BuiltinFunctionType.AddressOf:
                    {
                        OnAddressOf(node);
                        break;
                    }

                case BuiltinFunctionType.Eval:
                    {
                        OnEval(node);
                        break;
                    }

                case BuiltinFunctionType.InitValueType:
                    {
                        OnInitValueType(node);
                        break;
                    }

                case BuiltinFunctionType.Default:
                    {
                        EmitDefaultValue((IType)node.ExpressionType);
                        break;
                    }

                default:
                    {
                        NotImplemented(node, "BuiltinFunction: " + function.FunctionType);
                        break;
                    }
            }
        }

        private void OnInitValueType(MethodInvocationExpression node)
        {
            Debug.Assert(1 == node.Arguments.Count);

            Expression argument = node.Arguments[0];
            LoadAddressForInitObj(argument);
            var expressionType = GetExpressionType(argument);
            var type = GetSystemType(expressionType);
            Debug.Assert(type.IsValueType || (type is Microsoft.Cci.IGenericParameter && expressionType.IsValueType));
            _il.Emit(OperationCode.Initobj, type);
            PushVoid();
        }

        private void LoadAddressForInitObj(Expression argument)
        {
            IEntity entity = argument.Entity;
            switch (entity.EntityType)
            {
                case EntityType.Local:
                    {
                        InternalLocal local = (InternalLocal)entity;
                        LocalDefinition builder = local.LocalDefinition;
                        _il.Emit(OperationCode.Ldloca, builder);
                        break;
                    }
                case EntityType.Field:
                    {
                        EmitLoadFieldAddress(argument, (IField)entity);
                        break;
                    }
                default:
                    NotImplemented(argument, "__initobj__");
                    break;
            }
        }

        public override void OnMethodInvocationExpression(MethodInvocationExpression node)
        {
            IEntity entity = TypeSystemServices.GetEntity(node.Target);
            switch (entity.EntityType)
            {
                case EntityType.BuiltinFunction:
                    {
                        OnBuiltinFunction((BuiltinFunction)entity, node);
                        break;
                    }

                case EntityType.Method:
                    {
                        var methodInfo = (IMethod)entity;
                        if (node.Target.NodeType == NodeType.SuperLiteralExpression)
                            InvokeSuperMethod(methodInfo, node);
                        else
                            InvokeMethod(methodInfo, node);
                        break;
                    }

                case EntityType.Constructor:
                    {
                        IConstructor constructorInfo = (IConstructor)entity;
                        IMethodReference ci = GetConstructorInfo(constructorInfo);

                        if (NodeType.SuperLiteralExpression == node.Target.NodeType || node.Target.NodeType == NodeType.SelfLiteralExpression)
                        {
                            // super constructor call
                            _il.Emit(OperationCode.Ldarg_0);
                            PushArguments(constructorInfo, node.Arguments);
                            _il.Emit(OperationCode.Call, ci);
                            PushVoid();
                        }
                        else
                        {
                            PushArguments(constructorInfo, node.Arguments);
                            _il.Emit(OperationCode.Newobj, ci);

                            // constructor invocation resulting type is
                            PushType(constructorInfo.DeclaringType);
                        }
                        break;
                    }

                default:
                    {
                        NotImplemented(node, entity.ToString());
                        break;
                    }
            }
        }

        public override void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node)
        {
            EmitLoadLiteral(node.Value.Ticks);
            _il.Emit(OperationCode.Newobj, _timeSpanLongConstructor);
            PushType(TypeSystemServices.TimeSpanType);
        }

        public override void OnIntegerLiteralExpression(IntegerLiteralExpression node)
        {
            IType type = node.ExpressionType ?? TypeSystemServices.IntType;
            EmitLoadLiteral(type, node.Value);
            PushType(type);
        }

        public override void OnDoubleLiteralExpression(DoubleLiteralExpression node)
        {
            IType type = node.ExpressionType ?? TypeSystemServices.DoubleType;
            EmitLoadLiteral(type, node.Value);
            PushType(type);
        }

        private void EmitLoadLiteral(int i)
        {
            EmitLoadLiteral(TypeSystemServices.IntType, i);
        }

        private void EmitLoadLiteral(long l)
        {
            EmitLoadLiteral(TypeSystemServices.LongType, l);
        }

        private void EmitLoadLiteral(IType type, double d)
        {
            if (type == TypeSystemServices.SingleType)
            {
                if (d != 0)
                    _il.Emit(OperationCode.Ldc_R4, (float)d);
                else
                {
                    _il.Emit(OperationCode.Ldc_I4_0);
                    _il.Emit(OperationCode.Conv_R4);
                }
                return;
            }

            if (type == TypeSystemServices.DoubleType)
            {
                if (d != 0)
                    _il.Emit(OperationCode.Ldc_R8, d);
                else
                {
                    _il.Emit(OperationCode.Ldc_I4_0);
                    _il.Emit(OperationCode.Conv_R8);
                }
                return;
            }

            throw new InvalidOperationException(string.Format("`{0}' is not a literal", type));
        }

        private void EmitLoadLiteral(IType type, long l)
        {
            if (type.IsEnum)
                type = TypeSystemServices.Map(GetEnumUnderlyingType(type));

            if (!(IsInteger(type) || type == TypeSystemServices.CharType))
                throw new InvalidOperationException();

            var needsLongConv = true;
            switch (l)
            {
                case -1L:
                    {
                        if (IsLong(type) || type == TypeSystemServices.ULongType)
                        {
                            _il.Emit(OperationCode.Ldc_I8, -1L);
                            needsLongConv = false;
                        }
                        else
                            _il.Emit(OperationCode.Ldc_I4_M1);
                    }
                    break;
                case 0L:
                    _il.Emit(OperationCode.Ldc_I4_0);
                    break;
                case 1L:
                    _il.Emit(OperationCode.Ldc_I4_1);
                    break;
                case 2L:
                    _il.Emit(OperationCode.Ldc_I4_2);
                    break;
                case 3L:
                    _il.Emit(OperationCode.Ldc_I4_3);
                    break;
                case 4L:
                    _il.Emit(OperationCode.Ldc_I4_4);
                    break;
                case 5L:
                    _il.Emit(OperationCode.Ldc_I4_5);
                    break;
                case 6L:
                    _il.Emit(OperationCode.Ldc_I4_6);
                    break;
                case 7L:
                    _il.Emit(OperationCode.Ldc_I4_7);
                    break;
                case 8L:
                    _il.Emit(OperationCode.Ldc_I4_8);
                    break;
                default:
                    {
                        if (IsLong(type))
                        {
                            _il.Emit(OperationCode.Ldc_I8, l);
                            return;
                        }

                        if (l == (sbyte)l) //fits in an signed i1
                        {
                            _il.Emit(OperationCode.Ldc_I4_S, (sbyte)l);
                        }
                        else if (l == (int)l || l == (uint)l) //fits in an i4
                        {
                            if ((int)l == -1)
                                _il.Emit(OperationCode.Ldc_I4_M1);
                            else
                                _il.Emit(OperationCode.Ldc_I4, (int)l);
                        }
                        else
                        {
                            _il.Emit(OperationCode.Ldc_I8, l);
                            needsLongConv = false;
                        }
                    }
                    break;
            }

            if (needsLongConv && IsLong(type))
                _il.Emit(OperationCode.Conv_I8);
            else if (type == TypeSystemServices.ULongType)
                _il.Emit(OperationCode.Conv_U8);
        }

        private bool IsLong(IType type)
        {
            return type == TypeSystemServices.LongType;
        }

        public override void OnBoolLiteralExpression(BoolLiteralExpression node)
        {
            if (node.Value)
            {
                _il.Emit(OperationCode.Ldc_I4_1);
            }
            else
            {
                _il.Emit(OperationCode.Ldc_I4_0);
            }
            PushBool();
        }

        public override void OnHashLiteralExpression(HashLiteralExpression node)
        {
            _il.Emit(OperationCode.Newobj, _hashConstructor);

            var objType = TypeSystemServices.ObjectType;
            foreach (var pair in node.Items)
            {
                Dup();

                LoadExpressionWithType(objType, pair.First);
                LoadExpressionWithType(objType, pair.Second);
                _il.Emit(OperationCode.Callvirt, _hashAdd);
            }

            PushType(TypeSystemServices.HashType);
        }

        public override void OnGeneratorExpression(GeneratorExpression node)
        {
            NotImplemented(node, node.ToString());
        }

        public override void OnListLiteralExpression(ListLiteralExpression node)
        {
            if (node.Items.Count > 0)
            {
                EmitObjectArray(node.Items);
                _il.Emit(OperationCode.Ldc_I4_1);
                _il.Emit(OperationCode.Newobj, _listArrayBoolConstructor);
            }
            else
            {
                _il.Emit(OperationCode.Newobj, _listEmptyConstructor);
            }
            PushType(TypeSystemServices.ListType);
        }

        public override void OnArrayLiteralExpression(ArrayLiteralExpression node)
        {
            var type = (TypeSystem.IArrayType)node.ExpressionType;
            EmitArray(type.ElementType, node.Items);
            PushType(type);
        }

        public override void OnRELiteralExpression(RELiteralExpression node)
        {
            RegexOptions options = AstUtil.GetRegexOptions(node);

            _il.Emit(OperationCode.Ldstr, node.Pattern);
            if (options == RegexOptions.None)
            {
                _il.Emit(OperationCode.Newobj, _regexConstructor);
            }
            else
            {
                EmitLoadLiteral((int)options);
                _il.Emit(OperationCode.Newobj, _regexConstructorOptions);
            }

            PushType(node.ExpressionType);
        }

        public override void OnStringLiteralExpression(StringLiteralExpression node)
        {
            if (null == node.Value)
            {
                _il.Emit(OperationCode.Ldnull);
            }
            else if (0 != node.Value.Length)
            {
                _il.Emit(OperationCode.Ldstr, node.Value);
            }
            else /* force use of CLR-friendly string.Empty */
            {
                _il.Emit(OperationCode.Ldsfld, _stringEmpty);
            }
            PushType(TypeSystemServices.StringType);
        }

        public override void OnCharLiteralExpression(CharLiteralExpression node)
        {
            EmitLoadLiteral(node.Value[0]);
            PushType(TypeSystemServices.CharType);
        }

        public override void OnSlicingExpression(SlicingExpression node)
        {
            if (node.IsTargetOfAssignment())
                return;

            Visit(node.Target);
            var type = (TypeSystem.IArrayType)PopType();

            if (type.Rank == 1)
                LoadSingleDimensionalArrayElement(node, type);
            else
                LoadMultiDimensionalArrayElement(node, type);

            PushType(type.ElementType);
        }

        private void LoadMultiDimensionalArrayElement(SlicingExpression node, TypeSystem.IArrayType arrayType)
        {
            LoadArrayIndices(node);
            CallArrayGet(arrayType);
        }

	    private void CallArrayGet(IType arrayType)
	    {
	        var arrType = GetSystemType(arrayType);
            _il.Emit(OperationCode.Array_Get, arrType);
	    }

        private void CallArraySet(IType arrayType)
        {
            var arrType = GetSystemType(arrayType);
            _il.Emit(OperationCode.Array_Set, arrType);
        }

        private void CallArrayAddr(IType arrayType)
        {
            var arrType = GetSystemType(arrayType);
            _il.Emit(OperationCode.Array_Addr, arrType);
        }

        private void LoadArrayIndices(SlicingExpression node)
        {
            foreach (var index in node.Indices.Select(index => index.Begin))
                LoadIntExpression(index);
        }

        private void LoadSingleDimensionalArrayElement(SlicingExpression node, IType arrayType)
        {
            EmitNormalizedArrayIndex(node, node.Indices[0].Begin);

            var elementType = arrayType.ElementType;
            var opcode = GetLoadEntityOpCode(elementType);
            if (opcode == OperationCode.Ldelema)
            {
                var systemType = GetSystemType(elementType);
                _il.Emit(opcode, systemType);
                if (!IsByAddress(elementType))
                    _il.Emit(OperationCode.Ldobj, systemType);
            }
            else if (opcode == OperationCode.Ldelem)
                _il.Emit(opcode, GetSystemType(elementType));
            else
                _il.Emit(opcode);
        }

        private void EmitNormalizedArrayIndex(SlicingExpression sourceNode, Expression index)
        {
            bool isNegative = false;
            if (CanBeNegative(index, ref isNegative)
                && !_rawArrayIndexing
                && !AstAnnotations.IsRawIndexing(sourceNode))
            {
                if (isNegative)
                {
                    Dup();
                    _il.Emit(OperationCode.Ldlen);
                    LoadIntExpression(index);
                    _il.Emit(OperationCode.Add);
                }
                else
                {
                    Dup();
                    LoadIntExpression(index);
                    Call(_runtimeServicesNormalizeArrayIndex);
                }
            }
            else
                LoadIntExpression(index);
        }

        bool CanBeNegative(Expression expression, ref bool isNegative)
        {
            var integer = expression as IntegerLiteralExpression;
            if (integer != null)
            {
                if (integer.Value >= 0)
                    return false;
                isNegative = true;
            }
            return true;
        }

        private void LoadIntExpression(Expression expression)
        {
            LoadExpressionWithType(TypeSystemServices.IntType, expression);
        }

        public override void OnExpressionInterpolationExpression(ExpressionInterpolationExpression node)
        {
            Type stringBuilderType = typeof(StringBuilder);
            Expression arg0 = node.Expressions[0];
            IType argType = arg0.ExpressionType;

            /* if arg0 is a string, let's call StringBuilder constructor
			 * directly with the string */
            if ((typeof(StringLiteralExpression) == arg0.GetType()
                   && ((StringLiteralExpression)arg0).Value.Length > 0)
                || (typeof(StringLiteralExpression) != arg0.GetType()
                     && TypeSystemServices.StringType == argType))
            {
                Visit(arg0);
                PopType();
                _il.Emit(OperationCode.Newobj, _stringBuilderConstructorString);
            }
            else
            {
                _il.Emit(OperationCode.Newobj, _stringBuilderConstructor);
                arg0 = null; /* arg0 is not a string so we want it to be appended below */
            }

            string formatString;
            foreach (Expression arg in node.Expressions)
            {
                /* we do not need to append literal string.Empty
				 * or arg0 if it has been handled by ctor */
                if ((typeof(StringLiteralExpression) == arg.GetType()
                       && ((StringLiteralExpression)arg).Value.Length == 0)
                    || arg == arg0)
                {
                    continue;
                }

                formatString = arg["formatString"] as string; //annotation
                if (!string.IsNullOrEmpty(formatString))
                    _il.Emit(OperationCode.Ldstr, string.Format("{{0:{0}}}", formatString));

                Visit(arg);
                argType = PopType();

                if (!string.IsNullOrEmpty(formatString))
                {
                    EmitCastIfNeeded(TypeSystemServices.ObjectType, argType);
                    Call(StringFormat);
                }

                if (TypeSystemServices.StringType == argType || !string.IsNullOrEmpty(formatString))
                {
                    Call(_stringBuilderAppendString);
                }
                else
                {
                    EmitCastIfNeeded(TypeSystemServices.ObjectType, argType);
                    Call(_stringBuilderAppendObject);
                }
            }
            Call(_stringBuilderToString);
            PushType(TypeSystemServices.StringType);
        }

        private void LoadMemberTarget(Expression self, IMember member)
        {
            if (member.DeclaringType.IsValueType)
            {
                LoadAddress(self);
            }
            else
            {
                Visit(self);
                PopType();
            }
        }

        private void EmitLoadFieldAddress(Expression expression, IField field)
        {
            if (field.IsStatic)
            {
                _il.Emit(OperationCode.Ldsflda, GetFieldInfo(field));
            }
            else
            {
                LoadMemberTarget(((MemberReferenceExpression)expression).Target, field);
                _il.Emit(OperationCode.Ldflda, GetFieldInfo(field));
            }
        }

        private void EmitLoadField(Expression self, IField fieldInfo)
        {
            if (fieldInfo.IsStatic)
            {
                if (fieldInfo.IsLiteral)
                {
                    EmitLoadLiteralField(self, fieldInfo);
                }
                else
                {
                    if (fieldInfo.IsVolatile)
                        _il.Emit(OperationCode.Volatile_);
                    _il.Emit(IsByAddress(fieldInfo.Type) ? OperationCode.Ldsflda : OperationCode.Ldsfld,
                        GetFieldInfo(fieldInfo));
                }
            }
            else
            {
                LoadMemberTarget(self, fieldInfo);
                if (fieldInfo.IsVolatile)
                    _il.Emit(OperationCode.Volatile_);
                _il.Emit(IsByAddress(fieldInfo.Type) ? OperationCode.Ldflda : OperationCode.Ldfld,
                    GetFieldInfo(fieldInfo));
            }
            PushType(fieldInfo.Type);
        }

        object GetStaticValue(IField field)
        {
            InternalField internalField = field as InternalField;
            if (null != internalField)
            {
                return GetInternalFieldStaticValue(internalField);
            }
            return field.StaticValue;
        }

        private IMetadataConstant GetInternalFieldStaticValue(InternalField field)
        {
            return (IMetadataConstant)GetValue(field.Type, (Expression)field.StaticValue);
        }

        private void EmitLoadLiteralField(Node node, IField fieldInfo)
        {
            object value = GetStaticValue(fieldInfo);
            IType type = fieldInfo.Type;
            if (type.IsEnum)
            {
                Type underlyingType = GetEnumUnderlyingType(type);
                type = TypeSystemServices.Map(underlyingType);
                value = Convert.ChangeType(value, underlyingType);
            }

            var md = value as IMetadataConstant;
            if (null == value || (md != null && md.Value == null))
            {
                _il.Emit(OperationCode.Ldnull);
            }
            else if (type == TypeSystemServices.BoolType)
            {
                _il.Emit(((bool)value) ? OperationCode.Ldc_I4_1 : OperationCode.Ldc_I4_0);
            }
            else if (type == TypeSystemServices.StringType)
            {
                _il.Emit(OperationCode.Ldstr, value is string ? (string)value : (string)md.Value);
            }
            else if (type == TypeSystemServices.CharType)
            {
                EmitLoadLiteral(type, (long)(char)value);
            }
            else if (type == TypeSystemServices.IntType)
            {
                EmitLoadLiteral(type, (long)(int)value);
            }
            else if (type == TypeSystemServices.UIntType)
            {
                EmitLoadLiteral(type, unchecked((long)(uint)value));
            }
            else if (IsLong(type))
            {
                EmitLoadLiteral(type, (long)value);
            }
            else if (type == TypeSystemServices.ULongType)
            {
                EmitLoadLiteral(type, unchecked((long)(ulong)value));
            }
            else if (type == TypeSystemServices.SingleType)
            {
                EmitLoadLiteral(type, (double)(float)value);
            }
            else if (type == TypeSystemServices.DoubleType)
            {
                EmitLoadLiteral(type, (double)value);
            }
            else if (type == TypeSystemServices.SByteType)
            {
                EmitLoadLiteral(type, (long)(sbyte)value);
            }
            else if (type == TypeSystemServices.ByteType)
            {
                EmitLoadLiteral(type, (long)(byte)value);
            }
            else if (type == TypeSystemServices.ShortType)
            {
                EmitLoadLiteral(type, (long)(short)value);
            }
            else if (type == TypeSystemServices.UShortType)
            {
                EmitLoadLiteral(type, (long)(ushort)value);
            }
            else
            {
                NotImplemented(node, "Literal field type: " + type.ToString());
            }
        }

        public override void OnGenericReferenceExpression(GenericReferenceExpression node)
        {
            IEntity entity = TypeSystem.TypeSystemServices.GetEntity(node);
            switch (entity.EntityType)
            {
                case EntityType.Type:
                    {
                        EmitGetTypeFromHandle(GetSystemType(node));
                        break;
                    }

                case EntityType.Method:
                    {
                        node.Target.Accept(this);
                        break;
                    }

                default:
                    {
                        NotImplemented(node, entity.ToString());
                        break;
                    }
            }
        }

        public override void OnMemberReferenceExpression(MemberReferenceExpression node)
        {
            var tag = TypeSystemServices.GetEntity(node);
            switch (tag.EntityType)
            {
                case EntityType.Ambiguous:
                case EntityType.Method:
                    {
                        node.Target.Accept(this);
                        break;
                    }

                case EntityType.Field:
                    {
                        EmitLoadField(node.Target, (IField)tag);
                        break;
                    }

                case EntityType.Type:
                    {
                        EmitGetTypeFromHandle(GetSystemType(node));
                        break;
                    }

                default:
                    {
                        NotImplemented(node, tag.ToString());
                        break;
                    }
            }
        }

        private void LoadAddress(Expression expression)
        {
            if (expression.NodeType == NodeType.SelfLiteralExpression && expression.ExpressionType.IsValueType)
            {
                _il.Emit(OperationCode.Ldarg_0);
                return;
            }

            var entity = expression.Entity;
            if (entity != null)
            {
                switch (entity.EntityType)
                {
                    case EntityType.Local:
                        {
                            var local = ((InternalLocal)entity);
                            _il.Emit(!local.Type.IsPointer ? OperationCode.Ldloca : OperationCode.Ldloc, local.LocalDefinition);
                            return;
                        }

                    case EntityType.Parameter:
                        {
                            var param = (InternalParameter)entity;
                            if (param.Parameter.IsByRef)
                                LoadParam(param);
                            else
                                _il.Emit(OperationCode.Ldarga, (ParameterDefinition)GetBuilder(param.Parameter));
                            return;
                        }

                    case EntityType.Field:
                        {
                            var field = (IField)entity;
                            if (!field.IsLiteral)
                            {
                                EmitLoadFieldAddress(expression, field);
                                return;
                            }
                            break;
                        }
                }
            }

            if (IsValueTypeArraySlicing(expression))
            {
                LoadArrayElementAddress((SlicingExpression)expression);
                return;
            }

            Visit(expression);
            if (!AstUtil.IsIndirection(expression))
            {
                // declare local to hold value type
                var temp = DefineLocal(null, GetSystemType(PopType()) );
                _il.Emit(OperationCode.Stloc, temp);
                _il.Emit(OperationCode.Ldloca, temp);
            }
        }

        private void LoadArrayElementAddress(SlicingExpression slicing)
        {
            Visit(slicing.Target);
            var arrayType = (TypeSystem.IArrayType)PopType();

            if (arrayType.Rank == 1)
                LoadSingleDimensionalArrayElementAddress(slicing, arrayType);
            else
                LoadMultiDimensionalArrayElementAddress(slicing, arrayType);
        }

        private void LoadMultiDimensionalArrayElementAddress(SlicingExpression slicing, TypeSystem.IArrayType arrayType)
        {
            LoadArrayIndices(slicing);
            CallArrayAddr(arrayType);
        }

        private void LoadSingleDimensionalArrayElementAddress(SlicingExpression slicing, TypeSystem.IArrayType arrayType)
        {
            EmitNormalizedArrayIndex(slicing, slicing.Indices[0].Begin);
            _il.Emit(OperationCode.Ldelema, GetSystemType(arrayType.ElementType));
        }

        private static bool IsValueTypeArraySlicing(Expression expression)
        {
            var slicing = expression as SlicingExpression;
            if (slicing != null)
            {
                var type = (TypeSystem.IArrayType)slicing.Target.ExpressionType;
                return type.ElementType.IsValueType;
            }
            return false;
        }

        public override void OnSelfLiteralExpression(SelfLiteralExpression node)
        {
            LoadSelf(node);
        }

        public override void OnSuperLiteralExpression(SuperLiteralExpression node)
        {
            LoadSelf(node);
        }

        private void LoadSelf(Expression node)
        {
            _il.Emit(OperationCode.Ldarg_0);
            if (node.ExpressionType.IsValueType)
                _il.Emit(OperationCode.Ldobj, GetSystemType(node.ExpressionType));
            PushType(node.ExpressionType);
        }

        public override void OnNullLiteralExpression(NullLiteralExpression node)
        {
            _il.Emit(OperationCode.Ldnull);
            PushType(null);
        }

        public override void OnReferenceExpression(ReferenceExpression node)
        {
            var entity = TypeSystemServices.GetEntity(node);
            switch (entity.EntityType)
            {
                case EntityType.Local:
                    {
                        if (!AstUtil.IsIndirection(node.ParentNode))
                            LoadLocal((InternalLocal)entity);
                        else
                            LoadIndirectLocal((InternalLocal)entity);
                        break;
                    }

                case EntityType.Parameter:
                    {
                        var param = (InternalParameter)entity;
                        LoadParam(param);

                        if (param.Parameter.IsByRef)
                        {
                            var code = GetLoadRefParamCode(param.Type);
                            if (code == OperationCode.Ldobj)
                                _il.Emit(code, GetSystemType(param.Type));
                            else
                                _il.Emit(code);
                        }
                        PushType(param.Type);
                        break;
                    }

                case EntityType.Array:
                case EntityType.Type:
                    {
                        EmitGetTypeFromHandle(GetSystemType(node));
                        break;
                    }

                default:
                    {
                        NotImplemented(node, entity.ToString());
                        break;
                    }

            }
        }

        private void LoadParam(InternalParameter param)
        {
            int index = param.Index;

            switch (index)
            {
                case 0:
                    {
                        _il.Emit(OperationCode.Ldarg_0);
                        break;
                    }

                case 1:
                    {
                        _il.Emit(OperationCode.Ldarg_1);
                        break;
                    }

                case 2:
                    {
                        _il.Emit(OperationCode.Ldarg_2);
                        break;
                    }

                case 3:
                    {
                        _il.Emit(OperationCode.Ldarg_3);
                        break;
                    }

                default:
                    {
                        if (index < 256)
                        {
                            _il.Emit(OperationCode.Ldarg_S, index);
                        }
                        else
                        {
                            _il.Emit(OperationCode.Ldarg, index);
                        }
                        break;
                    }
            }
        }

        private void SetLocal(BinaryExpression node, InternalLocal tag, bool leaveValueOnStack)
        {
            if (AstUtil.IsIndirection(node.Left))
                _il.Emit(OperationCode.Ldloc, tag.LocalDefinition);

            node.Right.Accept(this); // leaves type on stack

            IType typeOnStack = null;

            if (leaveValueOnStack)
            {
                typeOnStack = PeekTypeOnStack();
                Dup();
            }
            else
            {
                typeOnStack = PopType();
            }

            if (!AstUtil.IsIndirection(node.Left))
                EmitAssignment(tag, typeOnStack);
            else
                EmitIndirectAssignment(tag, typeOnStack);
        }

        private void EmitAssignment(InternalLocal tag, IType typeOnStack)
        {
            // todo: assignment result must be type on the left in the
            // case of casting
            LocalDefinition local = tag.LocalDefinition;
            EmitCastIfNeeded(tag.Type, typeOnStack);
            _il.Emit(OperationCode.Stloc, local);
        }

        private void EmitIndirectAssignment(InternalLocal local, IType typeOnStack)
        {
            var elementType = local.Type.ElementType;
            EmitCastIfNeeded(elementType, typeOnStack);

            var code = GetStoreRefParamCode(elementType);
            if (code == OperationCode.Stobj)
                _il.Emit(code, GetSystemType(elementType));
            else
                _il.Emit(code);
        }

        private void SetField(Node sourceNode, IField field, Expression reference, Expression value, bool leaveValueOnStack)
        {
            OperationCode opSetField = OperationCode.Stsfld;
            if (!field.IsStatic)
            {
                opSetField = OperationCode.Stfld;
                if (null != reference)
                {
                    LoadMemberTarget(
                        ((MemberReferenceExpression)reference).Target,
                        field);
                }
            }

            LoadExpressionWithType(field.Type, value);

            LocalDefinition local = null;
            if (leaveValueOnStack)
            {
                Dup();
                local = DefineLocal(null, GetSystemType(field.Type) );
                _il.Emit(OperationCode.Stloc, local);
            }

            if (field.IsVolatile)
                _il.Emit(OperationCode.Volatile_);
            _il.Emit(opSetField, GetFieldInfo(field));

            if (leaveValueOnStack)
            {
                _il.Emit(OperationCode.Ldloc, local);
                PushType(field.Type);
            }
        }

        private void SetProperty(IProperty property, Expression reference, Expression value, bool leaveValueOnStack)
        {
            var callOpCode = OperationCode.Call;

            IMethodDefinition setMethod = GetMethodInfo(property.GetSetMethod());
            setMethod = ResolveGenericMethod(setMethod);
            IType targetType = null;
            if (null != reference)
            {
                if (!setMethod.IsStatic)
                {
                    Expression target = ((MemberReferenceExpression)reference).Target;
                    targetType = target.ExpressionType;
                    if (setMethod.ContainingTypeDefinition.IsValueType || targetType is TypeSystem.IGenericParameter)
                        LoadAddress(target);
                    else
                    {
                        callOpCode = GetCallOpCode(target, property.GetSetMethod());
                        target.Accept(this);
                        PopType();
                    }
                }
            }

            LoadExpressionWithType(property.Type, value);

            LocalDefinition local = null;
            if (leaveValueOnStack)
            {
                Dup();
                local = DefineLocal(null, GetSystemType(property.Type));
                _il.Emit(OperationCode.Stloc, local);
            }

            if (targetType is TypeSystem.IGenericParameter)
            {
                _il.Emit(OperationCode.Constrained_, GetSystemType(targetType));
                callOpCode = OperationCode.Callvirt;
            }

            _il.Emit(callOpCode, setMethod);

            if (leaveValueOnStack)
            {
                _il.Emit(OperationCode.Ldloc, local);
                PushType(property.Type);
            }
        }

        bool EmitDebugInfo(Node node)
        {
            if (!Parameters.Debug)
                return false;
            return EmitDebugInfo(node, node);
        }

        private const int _DBG_SYMBOLS_QUEUE_CAPACITY = 5;

        private readonly Queue<LexicalInfo> _dbgSymbols = new Queue<LexicalInfo>(_DBG_SYMBOLS_QUEUE_CAPACITY);

        private bool EmitDebugInfo(Node startNode, Node endNode)
        {
            LexicalInfo start = startNode.LexicalInfo;
            if (!start.IsValid) return false;
            if (_currentDocument == null) return false;

            // ensure there is no duplicate emitted
            if (_dbgSymbols.Contains(start))
            {
                Context.TraceInfo("duplicate symbol emit attempt for '{0}' : '{1}'.", start, startNode);
                return false;
            }
            if (_dbgSymbols.Count >= _DBG_SYMBOLS_QUEUE_CAPACITY) _dbgSymbols.Dequeue();
            _dbgSymbols.Enqueue(start);

            try
            {
                ISourceLocation loc = _currentDocument.GetLocation(start);
                _il.MarkSequencePoint(loc);
            }
            catch (Exception x)
            {
                Error(CompilerErrorFactory.InternalError(startNode, x));
                return false;
            }
            return true;
        }

        private void EmitNop()
        {
            _il.Emit(OperationCode.Nop);
        }

        private bool IsBoolOrInt(IType type)
        {
            return TypeSystemServices.BoolType == type ||
                TypeSystemServices.IntType == type;
        }

        private void PushArguments(IMethodBase entity, ExpressionCollection args)
        {
            var parameters = entity.GetParameters();
            for (var i = 0; i < args.Count; ++i)
            {
                var parameterType = parameters[i].Type;
                var arg = args[i];
                if (parameters[i].IsByRef)
                    LoadAddress(arg);
                else
                    LoadExpressionWithType(parameterType, arg);
            }
        }

        private void EmitObjectArray(ExpressionCollection items)
        {
            EmitArray(TypeSystemServices.ObjectType, items);
        }

	    private const int InlineArrayItemCountLimit = 3;

        private void EmitArray(IType type, ExpressionCollection items)
        {
            EmitLoadLiteral(items.Count);
            _il.Emit(OperationCode.Newarr, new VectorTypeReference { ElementType = GetSystemType(type) });

            if (items.Count == 0)
                return;

            var inlineStores = 0;
            if (items.Count > InlineArrayItemCountLimit && TypeSystemServices.IsPrimitiveNumber(type))
            {
                //packed array are only supported for a literal array of
                //an unique primitive type. check that all items are literal
                //and count number of actual stores in order to build/emit
                //a packed array only if is is an advantage
                foreach (Expression item in items)
                {
                    if ((item.NodeType != NodeType.IntegerLiteralExpression
                        && item.NodeType != NodeType.DoubleLiteralExpression)
                        || type != item.ExpressionType)
                    {
                        inlineStores = 0;
                        break;
                    }
                    if (IsZeroEquivalent(item))
                        continue;
                    ++inlineStores;
                }
            }

            if (inlineStores <= InlineArrayItemCountLimit)
                EmitInlineArrayInit(type, items);
            else
                EmitPackedArrayInit(type, items);
        }

        private void EmitInlineArrayInit(IType type, ExpressionCollection items)
        {
            OperationCode opcode = GetStoreEntityOpCode(type);
            for (int i = 0; i < items.Count; ++i)
            {
                if (IsNull(items[i]))
                    continue; //do not emit even if types are not the same (null is any)
                if (type == items[i].ExpressionType && IsZeroEquivalent(items[i]))
                    continue; //do not emit unnecessary init to zero
                StoreEntity(opcode, i, items[i], type);
            }
        }

	    private uint _staticDataOffset = 0;

	    private NamespaceTypeDefinition GetPackedArrayType(int size)
	    {
	        NamespaceTypeDefinition result;
	        if (!_packedArrayTypes.TryGetValue(size, out result))
	        {
	            var name = "$ArrayType$" + size.ToString();
                result = new NamespaceTypeDefinition
                {
                    ContainingUnitNamespace = _asmBuilder.UnitNamespaceRoot,
                    InternFactory = _host.InternFactory,
                    Name = _nameTable.GetNameFor(name),
                    Fields = new System.Collections.Generic.List<IFieldDefinition>(),
                    BaseClasses = new System.Collections.Generic.List<ITypeReference> { (GetTypeReference<ValueType>()) },
                    IsClass = true,
                    IsSealed = true,
                    IsValueType = true, 
                    Layout = LayoutKind.Explicit,
                    SizeOf = (uint)size
                };
                ((UnitNamespace)_asmBuilder.UnitNamespaceRoot).Members.Add(result);
                _asmBuilder.AllTypes.Add(result);
                _packedArrayTypes.Add(size, result);
	        }
	        return result;
	    }

	    private FieldDefinition DefineInitializedData(string name, byte[] ba)
	    {
            //there is no previously emitted bytearray to reuse, create it then
            var result = new FieldDefinition
            {
                Name = _nameTable.GetNameFor(name),
                InternFactory = _host.InternFactory,
                Type = GetPackedArrayType(ba.Length),
                ContainingTypeDefinition = _moduleClass,
                Visibility = TypeMemberVisibility.Private,
                IsStatic = true,
                FieldMapping = new SectionBlock
                {
                    PESectionKind = PESectionKind.ConstantData,
                    Data = new System.Collections.Generic.List<byte>(ba),
                    Size = (uint)ba.Length,
                    Offset = _staticDataOffset
                }
            };
            _staticDataOffset += (uint)ba.Length;
            _packedArrays.Add(ba, result);
            _moduleClass.Fields.Add(result);
	        return result;
	    }

        private void EmitPackedArrayInit(IType type, ExpressionCollection items)
        {
            var ba = CreateByteArrayFromLiteralCollection(type, items);
            if (null == ba)
            {
                EmitInlineArrayInit(type, items);
                return;
            }

            FieldDefinition fb;
            if (!_packedArrays.TryGetValue(ba, out fb))
            {
                //there is no previously emitted bytearray to reuse, create it then
                fb = DefineInitializedData(Context.GetUniqueName("newarr"), ba);
            }

            Dup(); //dup (newarr)
            _il.Emit(OperationCode.Ldtoken, fb);
            Call(_runtimeHelpersInitializeArray);
        }

	    private readonly Dictionary<byte[], FieldDefinition> _packedArrays = new Dictionary<byte[], FieldDefinition>(ValueTypeArrayEqualityComparer<byte>.Default);
        private readonly Dictionary<int, NamespaceTypeDefinition> _packedArrayTypes = new Dictionary<int, NamespaceTypeDefinition>();

	    private byte[] CreateByteArrayFromLiteralCollection(IType type, ExpressionCollection items)
        {
            using (var ms = new MemoryStream(items.Count * TypeSystemServices.SizeOf(type)))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    foreach (Expression item in items)
                    {
                        //TODO: BOO-1222 NumericLiteralExpression.GetValueAs<T>()
                        if (item.NodeType == NodeType.IntegerLiteralExpression)
                        {
                            IntegerLiteralExpression literal = (IntegerLiteralExpression)item;
                            if (type == TypeSystemServices.IntType)
                                writer.Write(Convert.ToInt32(literal.Value));
                            else if (type == TypeSystemServices.UIntType)
                                writer.Write(Convert.ToUInt32(literal.Value));
                            else if (IsLong(type))
                                writer.Write(Convert.ToInt64(literal.Value));
                            else if (type == TypeSystemServices.ULongType)
                                writer.Write(Convert.ToUInt64(literal.Value));
                            else if (type == TypeSystemServices.ShortType)
                                writer.Write(Convert.ToInt16(literal.Value));
                            else if (type == TypeSystemServices.UShortType)
                                writer.Write(Convert.ToUInt16(literal.Value));
                            else if (type == TypeSystemServices.ByteType)
                                writer.Write(Convert.ToByte(literal.Value));
                            else if (type == TypeSystemServices.SByteType)
                                writer.Write(Convert.ToSByte(literal.Value));
                            else if (type == TypeSystemServices.SingleType)
                                writer.Write(Convert.ToSingle(literal.Value));
                            else if (type == TypeSystemServices.DoubleType)
                                writer.Write(Convert.ToDouble(literal.Value));
                            else
                                return null;
                        }
                        else if (item.NodeType == NodeType.DoubleLiteralExpression)
                        {
                            DoubleLiteralExpression literal = (DoubleLiteralExpression)item;
                            if (type == TypeSystemServices.SingleType)
                                writer.Write(Convert.ToSingle(literal.Value));
                            else if (type == TypeSystemServices.DoubleType)
                                writer.Write(Convert.ToDouble(literal.Value));
                            else if (type == TypeSystemServices.IntType)
                                writer.Write(Convert.ToInt32(literal.Value));
                            else if (type == TypeSystemServices.UIntType)
                                writer.Write(Convert.ToUInt32(literal.Value));
                            else if (IsLong(type))
                                writer.Write(Convert.ToInt64(literal.Value));
                            else if (type == TypeSystemServices.ULongType)
                                writer.Write(Convert.ToUInt64(literal.Value));
                            else if (type == TypeSystemServices.ShortType)
                                writer.Write(Convert.ToInt16(literal.Value));
                            else if (type == TypeSystemServices.UShortType)
                                writer.Write(Convert.ToUInt16(literal.Value));
                            else if (type == TypeSystemServices.ByteType)
                                writer.Write(Convert.ToByte(literal.Value));
                            else if (type == TypeSystemServices.SByteType)
                                writer.Write(Convert.ToSByte(literal.Value));
                            else
                                return null;
                        }
                        else
                            return null;
                    }
                }
                return ms.ToArray();
            }
        }

        private bool IsInteger(IType type)
        {
            return TypeSystemServices.IsIntegerNumber(type);
        }

        private IMethodDefinition GetToDecimalConversionMethod(IType type)
        {
            var decimalType = GetTypeReference<decimal>();
            var toType = GetSystemType(type);
            var method = TypeHelper.GetMethod(decimalType, _nameTable.GetNameFor("op_Implicit"), toType);
            if (method == Dummy.MethodDefinition)
            {
                method = TypeHelper.GetMethod(decimalType, _nameTable.GetNameFor("op_Explicit"), toType);
                if (method == Dummy.MethodDefinition)
                {
                    NotImplemented(string.Format("Numeric promotion for {0} to decimal not implemented!", type));
                    return null; //unreachable; NotImplemented throws.  Just silencing a compiler warning
                }
            }
            return method;
        }

        private IMethodDefinition GetFromDecimalConversionMethod(IType type)
        {
            string toType = "To" + type.Name;

            var method =
                typeof(decimal).GetMethod(toType, new Type[] { typeof(decimal) });
            if (method == null)
            {
                NotImplemented(string.Format("Numeric promotion for decimal to {0} not implemented!", type));
            }
            return MethodOf<decimal>(method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray());
        }

        private OperationCode GetArithmeticOpCode(IType type, BinaryOperatorType op)
        {
            if (IsCheckedIntegerOperand(type))
            {
                switch (op)
                {
                    case BinaryOperatorType.Addition: return OperationCode.Add_Ovf;
                    case BinaryOperatorType.Subtraction: return OperationCode.Sub_Ovf;
                    case BinaryOperatorType.Multiply: return OperationCode.Mul_Ovf;
                    case BinaryOperatorType.Division: return OperationCode.Div;
                    case BinaryOperatorType.Modulus: return OperationCode.Rem;
                }
            }
            else
            {
                switch (op)
                {
                    case BinaryOperatorType.Addition: return OperationCode.Add;
                    case BinaryOperatorType.Subtraction: return OperationCode.Sub;
                    case BinaryOperatorType.Multiply: return OperationCode.Mul;
                    case BinaryOperatorType.Division: return OperationCode.Div;
                    case BinaryOperatorType.Modulus: return OperationCode.Rem;
                }
            }
            throw new ArgumentException("op");
        }

        private OperationCode GetLoadEntityOpCode(IType type)
        {
            if (IsByAddress(type))
                return OperationCode.Ldelema;

            if (!type.IsValueType)
            {
                return type is TypeSystem.IGenericParameter
                    ? OperationCode.Ldelem
                    : OperationCode.Ldelem_Ref;
            }

            if (type.IsEnum)
            {
                type = TypeSystemServices.Map(GetEnumUnderlyingType(type));
            }

            if (TypeSystemServices.IntType == type)
            {
                return OperationCode.Ldelem_I4;
            }
            if (TypeSystemServices.UIntType == type)
            {
                return OperationCode.Ldelem_U4;
            }
            if (IsLong(type))
            {
                return OperationCode.Ldelem_I8;
            }
            if (TypeSystemServices.SByteType == type)
            {
                return OperationCode.Ldelem_I1;
            }
            if (TypeSystemServices.ByteType == type)
            {
                return OperationCode.Ldelem_U1;
            }
            if (TypeSystemServices.ShortType == type ||
                TypeSystemServices.CharType == type)
            {
                return OperationCode.Ldelem_I2;
            }
            if (TypeSystemServices.UShortType == type)
            {
                return OperationCode.Ldelem_U2;
            }
            if (TypeSystemServices.SingleType == type)
            {
                return OperationCode.Ldelem_R4;
            }
            if (TypeSystemServices.DoubleType == type)
            {
                return OperationCode.Ldelem_R8;
            }
            //NotImplemented("LoadEntityOpCode(" + tag + ")");
            return OperationCode.Ldelema;
        }

        private OperationCode GetStoreEntityOpCode(IType tag)
        {
            if (tag.IsValueType || tag is TypeSystem.IGenericParameter)
            {
                if (tag.IsEnum)
                {
                    tag = TypeSystemServices.Map(GetEnumUnderlyingType(tag));
                }

                if (TypeSystemServices.IntType == tag ||
                    TypeSystemServices.UIntType == tag)
                {
                    return OperationCode.Stelem_I4;
                }
                else if (IsLong(tag) ||
                    TypeSystemServices.ULongType == tag)
                {
                    return OperationCode.Stelem_I8;
                }
                else if (TypeSystemServices.ShortType == tag ||
                    TypeSystemServices.CharType == tag)
                {
                    return OperationCode.Stelem_I2;
                }
                else if (TypeSystemServices.ByteType == tag ||
                    TypeSystemServices.SByteType == tag)
                {
                    return OperationCode.Stelem_I1;
                }
                else if (TypeSystemServices.SingleType == tag)
                {
                    return OperationCode.Stelem_R4;
                }
                else if (TypeSystemServices.DoubleType == tag)
                {
                    return OperationCode.Stelem_R8;
                }
                return OperationCode.Stobj;
            }

            return OperationCode.Stelem_Ref;
        }

        private OperationCode GetLoadRefParamCode(IType tag)
        {
            if (tag.IsValueType)
            {
                if (tag.IsEnum)
                {
                    tag = TypeSystemServices.Map(GetEnumUnderlyingType(tag));
                }
                if (TypeSystemServices.IntType == tag)
                {
                    return OperationCode.Ldind_I4;
                }
                if (IsLong(tag) ||
                    TypeSystemServices.ULongType == tag)
                {
                    return OperationCode.Ldind_I8;
                }
                if (TypeSystemServices.ByteType == tag)
                {
                    return OperationCode.Ldind_U1;
                }
                if (TypeSystemServices.ShortType == tag ||
                    TypeSystemServices.CharType == tag)
                {
                    return OperationCode.Ldind_I2;
                }
                if (TypeSystemServices.SingleType == tag)
                {
                    return OperationCode.Ldind_R4;
                }
                if (TypeSystemServices.DoubleType == tag)
                {
                    return OperationCode.Ldind_R8;
                }
                if (TypeSystemServices.UShortType == tag)
                {
                    return OperationCode.Ldind_U2;
                }
                if (TypeSystemServices.UIntType == tag)
                {
                    return OperationCode.Ldind_U4;
                }

                return OperationCode.Ldobj;
            }
            return OperationCode.Ldind_Ref;
        }

        OperationCode GetStoreRefParamCode(IType tag)
        {
            if (tag.IsValueType)
            {
                if (tag.IsEnum)
                {
                    tag = TypeSystemServices.Map(GetEnumUnderlyingType(tag));
                }
                if (TypeSystemServices.IntType == tag
                    || TypeSystemServices.UIntType == tag)
                {
                    return OperationCode.Stind_I4;
                }
                if (IsLong(tag)
                    || TypeSystemServices.ULongType == tag)
                {
                    return OperationCode.Stind_I8;
                }
                if (TypeSystemServices.ByteType == tag)
                {
                    return OperationCode.Stind_I1;
                }
                if (TypeSystemServices.ShortType == tag ||
                    TypeSystemServices.CharType == tag)
                {
                    return OperationCode.Stind_I2;
                }
                if (TypeSystemServices.SingleType == tag)
                {
                    return OperationCode.Stind_R4;
                }
                if (TypeSystemServices.DoubleType == tag)
                {
                    return OperationCode.Stind_R8;
                }

                return OperationCode.Stobj;
            }
            return OperationCode.Stind_Ref;
        }

        private bool IsAssignableFrom(IType expectedType, IType actualType)
        {
            return (IsPtr(expectedType) && IsPtr(actualType))
                || TypeCompatibilityRules.IsAssignableFrom(expectedType, actualType);
        }

        private bool IsPtr(IType type)
        {
            return (type == TypeSystemServices.IntPtrType)
                || (type == TypeSystemServices.UIntPtrType);
        }

        private void EmitCastIfNeeded(IType expectedType, IType actualType)
        {
            if (actualType == null) // see NullLiteralExpression
                return;

            if (expectedType == actualType)
                return;

            if (expectedType.IsPointer || actualType.IsPointer) //no cast needed for addresses
                return;

            if (IsAssignableFrom(expectedType, actualType))
            {
                EmitBoxIfNeeded(expectedType, actualType);
                return;
            }

            var method = TypeSystemServices.FindImplicitConversionOperator(actualType, expectedType)
                         ?? TypeSystemServices.FindExplicitConversionOperator(actualType, expectedType);
            if (method != null)
            {
                EmitBoxIfNeeded(method.GetParameters()[0].Type, actualType);
                Call(GetMethodInfo(method));
                return;
            }

            if (expectedType is TypeSystem.IGenericParameter)
            {
                // Since expected type is a generic parameter, we don't know whether to emit
                // an unbox opcode or a castclass opcode; so we emit an unbox.any opcode which
                // works as either of those at runtime
                _il.Emit(OperationCode.Unbox_Any, GetSystemType(expectedType));
                return;
            }

            if (expectedType.IsValueType)
            {
                if (!actualType.IsValueType)
                {
                    // To get a value type out of a reference type we emit an unbox opcode
                    EmitUnbox(expectedType);
                    return;
                }

                // numeric promotion
                if (TypeSystemServices.DecimalType == expectedType)
                {
                    Call(GetToDecimalConversionMethod(actualType));
                }
                else if (TypeSystemServices.DecimalType == actualType)
                {
                    Call(GetFromDecimalConversionMethod(expectedType));
                }
                else
                {
                    //we need to get the real underlying type here and no earlier
                    //(because cause enum casting from int can occur [e.g enums-13])
                    if (actualType.IsEnum)
                        actualType = TypeSystemServices.Map(GetEnumUnderlyingType(actualType));
                    if (expectedType.IsEnum)
                        expectedType = TypeSystemServices.Map(GetEnumUnderlyingType(expectedType));
                    if (actualType != expectedType) //do we really need conv?
                        _il.Emit(GetNumericPromotionOpCode(expectedType));
                }
                return;
            }

            EmitRuntimeCoercionIfNeeded(expectedType, actualType);
        }

        private void EmitRuntimeCoercionIfNeeded(IType expectedType, IType actualType)
        {
            // In order to cast to a reference type we emit a castclass opcode
            Context.TraceInfo("castclass: expected type='{0}', type on stack='{1}'", expectedType, actualType);
            var expectedSystemType = GetSystemType(expectedType);
            if (TypeSystemServices.IsSystemObject(actualType))
            {
                Dup();
                Isinst(expectedSystemType);

                var skipCoercion = new ILGeneratorLabel();
                _il.Emit(OperationCode.Brtrue, skipCoercion);

                EmitGetTypeFromHandle(expectedSystemType); PopType();
                Call(RuntimeServices_Coerce);

                _il.MarkLabel(skipCoercion);
            }
            Castclass(expectedSystemType);
        }

        private void Call(IMethodReference method)
        {
            _il.Emit(OperationCode.Call, method);
        }

        private void Castclass(ITypeReference expectedSystemType)
        {
            _il.Emit(OperationCode.Castclass, expectedSystemType);
        }

        private IMethodDefinition _runtimeServicesCoerce;

        private IMethodDefinition RuntimeServices_Coerce
        {
            get
            {
                if (_runtimeServicesCoerce != null) return _runtimeServicesCoerce;
                MethodOf<RuntimeServices>("Coerce", typeof(object), typeof(Type));
                return _runtimeServicesCoerce = MethodOf<RuntimeServices>("Coerce", typeof(object), typeof(Type));
            }
        }

        private void EmitBoxIfNeeded(IType expectedType, IType actualType)
        {
            if ((actualType.IsValueType && !expectedType.IsValueType)
                || (actualType is TypeSystem.IGenericParameter && !(expectedType is TypeSystem.IGenericParameter)))
                EmitBox(actualType);
        }

        private void EmitBox(IType type)
        {
            _il.Emit(OperationCode.Box, GetSystemType(type));
        }

        private void EmitUnbox(IType expectedType)
        {
            var unboxMethod = UnboxMethodFor(expectedType);
            if (null != unboxMethod)
            {
                Call(unboxMethod);
            }
            else
            {
                var type = GetSystemType(expectedType);
                _il.Emit(OperationCode.Unbox, type);
                _il.Emit(OperationCode.Ldobj, type);
            }
        }

        private IMethodReference GetGenericMethod(IGenericTypeInstance type, IMethodReference method)
        {
            var candidates = type.GetMembersNamed(method.Name, false)
                .OfType<IMethodDefinition>()
                .Where(md => md.ParameterCount == method.ParameterCount && md.GenericParameterCount == method.GenericParameterCount)
                .ToArray();
            var result = candidates.SingleOrDefault(c => MemberHelper.GenericMethodSignaturesAreEqual(method, c));
            return result;
        }

        private IMethodDefinition GenericMethodOf(ITypeDefinition typeRef, string name, Type[] args, Type[] genericArgs)
	    {
	        var genericTypes = genericArgs.Select(GetTypeReference).ToArray();
            var argTypes = args.Select(GetTypeReference).ToArray();
	        var baseMethodCandidates = typeRef.GetMembersNamed(_nameTable.GetNameFor(name), false)
	            .OfType<IMethodDefinition>()
	            .Where(mr => mr.GenericParameterCount == genericArgs.Length)
                .ToArray();
	        var constructedCandidates = baseMethodCandidates
	            .Select(c => new GenericMethodInstance(c, genericTypes, _host.InternFactory))
	            .ToArray();
	        return constructedCandidates.Single(c => ParamsMatch(c, argTypes));
	    }

        private IMethodDefinition MethodOf(ITypeDefinition typeRef, string name, params Type[] args)
        {
            var argTypes = args.Select(GetTypeReference).ToArray();
            if (args.Any(t => t.IsByRef))
                return CciLookupHelper.GetMethod(typeRef, _nameTable.GetNameFor(name), argTypes);
            return TypeHelper.GetMethod(typeRef, _nameTable.GetNameFor(name), argTypes);
        }

        private IMethodDefinition MethodOf<T>(string name, params Type[] args)
        {
            var typeRef = GetTypeReference(typeof(T));
            return TypeHelper.GetMethod(typeRef, _nameTable.GetNameFor(name), args.Select(GetTypeReference).ToArray());
        }

        private IMethodDefinition ConstructorOf<T>(params Type[] args)
        {
            return MethodOf<T>(".ctor", args);
        }

        private IMethodDefinition ConstructorOf(ITypeDefinition typeRef, params Type[] args)
        {
            return MethodOf(typeRef, ".ctor", args);
        }

        private IPropertyDefinition PropertyOf<T>(string name)
        {
            var typeRef = GetTypeReference(typeof(T));
            return TypeHelper.GetProperty(typeRef, _nameTable.GetNameFor(name));
        }

        private IMethodDefinition UnboxMethodFor(IType type)
        {
            var runtimeServicesType = GetTypeReference<RuntimeServices>();
            if (type == TypeSystemServices.ByteType) return MethodOf(runtimeServicesType, "UnboxByte", typeof(object));
            if (type == TypeSystemServices.SByteType) return MethodOf(runtimeServicesType, "UnboxSByte", typeof(object));
            if (type == TypeSystemServices.ShortType) return MethodOf(runtimeServicesType, "UnboxInt16", typeof(object));
            if (type == TypeSystemServices.UShortType) return MethodOf(runtimeServicesType, "UnboxUInt16", typeof(object));
            if (type == TypeSystemServices.IntType) return MethodOf(runtimeServicesType, "UnboxInt32", typeof(object));
            if (type == TypeSystemServices.UIntType) return MethodOf(runtimeServicesType, "UnboxUInt32", typeof(object));
            if (IsLong(type)) return MethodOf(runtimeServicesType, "UnboxInt64", typeof(object));
            if (type == TypeSystemServices.ULongType) return MethodOf(runtimeServicesType, "UnboxUInt64", typeof(object));
            if (type == TypeSystemServices.SingleType) return MethodOf(runtimeServicesType, "UnboxSingle", typeof(object));
            if (type == TypeSystemServices.DoubleType) return MethodOf(runtimeServicesType, "UnboxDouble", typeof(object));
            if (type == TypeSystemServices.DecimalType) return MethodOf(runtimeServicesType, "UnboxDecimal", typeof(object));
            if (type == TypeSystemServices.BoolType) return MethodOf(runtimeServicesType, "UnboxBoolean", typeof(object));
            if (type == TypeSystemServices.CharType) return MethodOf(runtimeServicesType, "UnboxChar", typeof(object));
            return null;
        }

        OperationCode GetNumericPromotionOpCode(IType type)
        {
            return NumericPromotionOpcodeFor(TypeCodeFor(type), _checked);
        }

        private static OperationCode NumericPromotionOpcodeFor(TypeCode typeCode, bool @checked)
        {
            switch (typeCode)
            {
                case TypeCode.SByte:
                    return @checked ? OperationCode.Conv_Ovf_I1 : OperationCode.Conv_I1;
                case TypeCode.Byte:
                    return @checked ? OperationCode.Conv_Ovf_U1 : OperationCode.Conv_U1;
                case TypeCode.Int16:
                    return @checked ? OperationCode.Conv_Ovf_I2 : OperationCode.Conv_I2;
                case TypeCode.UInt16:
                case TypeCode.Char:
                    return @checked ? OperationCode.Conv_Ovf_U2 : OperationCode.Conv_U2;
                case TypeCode.Int32:
                    return @checked ? OperationCode.Conv_Ovf_I4 : OperationCode.Conv_I4;
                case TypeCode.UInt32:
                    return @checked ? OperationCode.Conv_Ovf_U4 : OperationCode.Conv_U4;
                case TypeCode.Int64:
                    return @checked ? OperationCode.Conv_Ovf_I8 : OperationCode.Conv_I8;
                case TypeCode.UInt64:
                    return @checked ? OperationCode.Conv_Ovf_U8 : OperationCode.Conv_U8;
                case TypeCode.Single:
                    return OperationCode.Conv_R4;
                case TypeCode.Double:
                    return OperationCode.Conv_R8;
                default:
                    throw new ArgumentException(typeCode.ToString());
            }
        }

        private static TypeCode TypeCodeFor(IType type)
        {
            var externalType = type as ExternalType;
            if (externalType != null)
                return Type.GetTypeCode(externalType.ActualType);
            return TypeCode.Object;
        }

        private void StoreEntity(OperationCode opcode, int index, Expression value, IType elementType)
        {
            // array reference
            Dup();
            EmitLoadLiteral(index); // element index

            bool stobj = IsStobj(opcode); // value type sequence?
            if (stobj)
            {
                var systemType = GetSystemType(elementType);
                _il.Emit(OperationCode.Ldelema, systemType);
                LoadExpressionWithType(elementType, value); // might need to cast to decimal
                _il.Emit(opcode, systemType);
            }
            else
            {
                LoadExpressionWithType(elementType, value);
                _il.Emit(opcode);
            }
        }

        private void Dup()
        {
            _il.Emit(OperationCode.Dup);
        }

        private static bool IsStobj(OperationCode code)
        {
            return code == OperationCode.Stobj;
        }

        private void DefineAssemblyAttributes()
        {
            foreach (var attribute in _assemblyAttributes)
            {
                _asmBuilder.AssemblyAttributes.Add(GetCustomAttributeBuilder(attribute));
            }
        }

        private CustomAttribute CreateDebuggableAttribute()
	    {
	        var ctor = ConstructorOf<DebuggableAttribute>(typeof(DebuggableAttribute.DebuggingModes));
	        return new CustomAttribute {
                Constructor = ctor,
                Arguments = new System.Collections.Generic.List<IMetadataExpression>
                {
                    new MetadataConstant
                    {
                        Type = GetTypeReference<DebuggableAttribute.DebuggingModes>(),
                        Value = DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.DisableOptimizations
                    }
                }
	        };
	    }

	    private CustomAttribute CreateRuntimeCompatibilityAttribute()
        {
            return new CustomAttribute{
                Constructor = ConstructorOf<System.Runtime.CompilerServices.RuntimeCompatibilityAttribute>(),
                NamedArguments = new System.Collections.Generic.List<IMetadataNamedArgument>
                {
                    new MetadataNamedArgument
                    {
                        ArgumentName = _nameTable.GetNameFor("WrapNonExceptionThrows"),
                        ArgumentValue = new MetadataConstant
                        {
                            Type = GetTypeReference<bool>(),
                            Value = true
                        },
                        Type = GetTypeReference<bool>()
                    }
                }
            };
        }

        private CustomAttribute CreateUnverifiableCodeAttribute()
        {
            return new CustomAttribute
            {
                Constructor = ConstructorOf<UnverifiableCodeAttribute>(),
            };
        }

        private void DefineEntryPoint()
        {
            if (Context.Parameters.GenerateInMemory)
            {
                Context.GeneratedAssemblyCci = _asmBuilder;
            }

            if (CompilerOutputType.Library != Parameters.OutputType)
            {
                Method method = ContextAnnotations.GetEntryPoint(Context);
                if (method != null)
                {
                    var methodTypeNameKey = _nameTable.GetNameFor(method.DeclaringType.Name).UniqueKey;
                    MethodDefinition entryPoint = Context.Parameters.GenerateInMemory
                        ? _asmBuilder.AllTypes.Single(t => t.Name.UniqueKey == methodTypeNameKey).GetMembersNamed(_nameTable.GetNameFor(method.Name), false).OfType<MethodDefinition>().Single(m => m.IsStatic)
                        : GetMethodBuilder(method);
                    _asmBuilder.EntryPoint = entryPoint;
                }
                else
                {
                    Errors.Add(CompilerErrorFactory.NoEntryPoint());
                }
            }
        }

        private void DefineModuleConstructor()
        {
            if (_moduleConstructorMethods.Count == 0)
                return;

            var mb = new MethodDefinition
            {
                InternFactory = _host.InternFactory,
                ContainingTypeDefinition = _moduleClass,
                IsStatic = true,
                IsSpecialName = true,
                IsRuntimeSpecial = true,
                Name = _nameTable.GetNameFor(".cctor")
            };
            _moduleClass.Methods.Add(mb);

            Method m = CodeBuilder.CreateMethod(".cctor", TypeSystemServices.VoidType, TypeMemberModifiers.Static);
            foreach (var reference in _moduleConstructorMethods.OrderBy(reference => (int)reference["Ordering"]))
                m.Body.Add(CodeBuilder.CreateMethodInvocation((IMethod)reference.Entity));

            EmitMethod(m, mb);
        }

        private readonly Dictionary<Node, object> _builders = new Dictionary<Node, object>();

        private void SetBuilder(Node node, object builder)
        {
            if (null == builder)
            {
                throw new ArgumentNullException("builder");
            }
            _builders[node] = builder;
        }

	    private object GetBuilder(Node node)
        {
            return _builders[node];
        }

        internal NamedTypeDefinition GetTypeBuilder(Node node)
        {
            return (NamedTypeDefinition)_builders[node];
        }

        private PropertyDefinition GetPropertyBuilder(Node node)
        {
            return (PropertyDefinition)_builders[node];
        }

        private FieldDefinition GetFieldBuilder(Node node)
        {
            return (FieldDefinition)_builders[node];
        }

	    private MethodDefinition GetMethodBuilder(Method method)
        {
            return (MethodDefinition)_builders[method];
        }

	    private MethodDefinition GetConstructorBuilder(Method method)
        {
            return (MethodDefinition)_builders[method];
        }

	    private LocalDefinition GetLocalBuilder(Node local)
        {
            return GetInternalLocal(local).LocalDefinition;
        }

	    private IFieldDefinition GetFieldInfo(IField tag)
        {
            // If field is external, get its existing IFieldDefinition
            var external = tag as ExternalField;
            if (null != external)
            {
                var baseType = GetSystemType(external.DeclaringType);
                return TypeHelper.GetField(baseType.ResolvedType, _nameTable.GetNameFor(tag.Name));
            }

            // If field is mapped from a generic type, get its mapped IFieldDefinition
            // on the constructed type
            GenericMappedField mapped = tag as GenericMappedField;
            if (mapped != null)
            {
                return GetMappedFieldInfo(mapped.DeclaringType, mapped.SourceMember);
            }

            if (tag.DeclaringType.GenericInfo != null && tag.DeclaringType.ConstructedInfo == null)
                return GetSelfMappedField(tag);

            // If field is internal, get its FieldDefinition
            return GetFieldBuilder(((InternalField)tag).Field);
        }

        private readonly Dictionary<IType, ITypeDefinition> _selfMap = new Dictionary<IType, ITypeDefinition>();
        private readonly Dictionary<IField, IFieldDefinition> _selfFieldMap = new Dictionary<IField, IFieldDefinition>();

        private ITypeDefinition GetSelfMappedClass(ITypeDefinition value)
        {
            ITypeDefinition result;
            Debug.Assert(value.IsGeneric || IsNestedGeneric(value));
            if (IsNestedGeneric(value))
            {
                var parent = GetSelfMappedClass(((INestedTypeDefinition)value).ContainingTypeDefinition);
                result =
                    new Microsoft.Cci.Immutable.NestedTypeReference(_host, parent,
                        ((INamedTypeDefinition)value).Name, value.GenericParameterCount, value.IsEnum, value.IsValueType, true).ResolvedType;
            }
            else result = value;
            if (result.IsGeneric)
            {
                result = GenericTypeInstance.GetGenericTypeInstance((INamedTypeDefinition)result,
                    value.GenericParameters,
                    _host.InternFactory);
            }
            return result;
        }

	    private static bool IsNestedGeneric(ITypeDefinition value)
	    {
	        var nested = value as INestedTypeDefinition;
	        while (nested != null)
	        {
	            var parent = nested.ContainingTypeDefinition;
	            if (parent.IsGeneric)
	                return true;
	            nested = parent as INestedTypeDefinition;
	        }
	        return false;
	    }

	    private IFieldDefinition GetSelfMappedField(IField tag)
	    {
	        IFieldDefinition result;
	        if (!_selfFieldMap.TryGetValue(tag, out result))
	        {
	            var genericParent = tag.DeclaringType;
	            ITypeDefinition genericParentType;
	            if (!_selfMap.TryGetValue(genericParent, out genericParentType))
	            {
	                var gpBuilder = GetTypeBuilder(((AbstractInternalType) genericParent).TypeDefinition);
	                genericParentType = GetSelfMappedClass(gpBuilder);
	                _selfMap.Add(genericParent, genericParentType);
	            }
	            result = GetMappedFieldInfo(genericParentType, GetFieldBuilder(((InternalField)tag).Field));
	            _selfFieldMap.Add(tag, result);
	        }
	        return result;
	    }

	    private IMethodDefinition GetMethodInfo(IMethod entity)
        {
            // If method is external, get its existing MethodDefinition
            var external = entity as ExternalMethod;
            if (external != null)
            {
                var mi = external.MethodInfo;
                if (mi.IsGenericMethod)
                    return GenericMethodOf(GetTypeReference(mi.DeclaringType), mi.Name, mi.GetParameters().Select(p => p.ParameterType).ToArray(), mi.GetGenericArguments());
                return MethodOf(GetTypeReference(mi.DeclaringType), mi.Name, mi.GetParameters().Select(p => p.ParameterType).ToArray());
            }

            // If method is a constructed generic method, get its MethodDefinition from its definition
            if (entity is GenericConstructedMethod)
                return GetConstructedMethodInfo(entity.ConstructedInfo);

            // If method is mapped from a generic type, get its MethodDefinition on the constructed type
            var mapped = entity as GenericMappedMethod;
            if (mapped != null)
                return GetMappedMethodInfo(mapped.DeclaringType, mapped.SourceMember);

            // If method is internal, get its MethodDefinition
            return GetMethodBuilder(((InternalMethod)entity).Method);
        }

	    private bool ParamsMatch(IMethodDefinition mb, params Type[] types)
	    {
	        var args = mb.Parameters.ToArray();
	        if (args.Length != types.Length)
	            return false;
	        var intern = _host.InternFactory;
	        return args.Zip(types, Tuple.Create)
	            .All(p => intern.GetTypeReferenceInternedKey(p.Item1.Type) == intern.GetTypeReferenceInternedKey(GetTypeReference(p.Item2)));
	    }

        private bool ParamsMatch(IMethodDefinition mb, params ITypeDefinition[] types)
        {
            var args = mb.Parameters.ToArray();
            if (args.Length != types.Length)
                return false;
            var intern = _host.InternFactory;
            return args.Zip(types, Tuple.Create)
                .All(p => intern.GetTypeReferenceInternedKey(p.Item1.Type) == intern.GetTypeReferenceInternedKey(p.Item2));
        }

        private IMethodReference GetConstructorInfo(IConstructor entity)
        {
            // If constructor is external, get its existing IMethodReference
            var external = entity as ExternalConstructor;
            if (null != external)
            {
                var ci = (System.Reflection.ConstructorInfo) external.MethodInfo;
                return ConstructorOf(GetTypeReference(ci.DeclaringType), ci.GetParameters().Select(p => p.ParameterType).ToArray());
            }

            // If constructor is mapped from a generic type, get its IMethodReference on the constructed type
            var mapped = entity as GenericMappedConstructor;
            if (mapped != null)
            {
                var baseGeneric = GetConstructorInfo((IConstructor) mapped.SourceMember);
                var declaringType = GetSystemType(mapped.DeclaringType);
                var baseType = declaringType as IGenericTypeInstance;
                if (baseType != null)
                {
                    var result = GetGenericMethod(baseType, baseGeneric);
                    if (result == null)
                    {
                        var mapping = GetAllGenericParameters(baseType.GenericType.ResolvedType)
                            .Zip(baseType.GenericArguments, (p, a) => Tuple.Create(p.Name.UniqueKey, a.ResolvedType))
                            .ToDictionary(t => t.Item1, t => t.Item2);
                        var spec = new GenericMethodSpecializer(_host,
                            mapping);
                        var specialized = spec.Substitute(baseGeneric);
                        result = GetGenericMethod(baseType, specialized);
                        if (result == null)
                        {
                            throw new NotSupportedException("Unable to lookup generic constructor");
                        }
                    }
                    return result;
                }
                return TypeHelper.GetMethod(declaringType.ResolvedType, baseGeneric, true);
            }

            // If constructor is internal, get its MethodDefinition
            return GetConstructorBuilder(((InternalMethod)entity).Method);
        }

	    private IEnumerable<IGenericTypeParameter> GetAllGenericParameters(INamedTypeDefinition typeDef)
	    {
	        var result = typeDef.GenericParameters;
	        var nested = typeDef as INestedTypeDefinition;
	        if (nested != null)
	            result = GetAllGenericParameters((INamedTypeDefinition) nested.ContainingType.ResolvedType).Concat(result);
	        return result;
	    }

	    /// <summary>
        /// Retrieves the MethodDefinition for a generic constructed method.
        /// </summary>
        private IMethodDefinition GetConstructedMethodInfo(IConstructedMethodInfo constructedInfo)
        {
            ITypeReference[] arguments = Array.ConvertAll(constructedInfo.GenericArguments, GetSystemType);
            var baseGeneric = GetMethodInfo(constructedInfo.GenericDefinition);
            var gmi = baseGeneric as IGenericMethodInstance;
            if (gmi != null)
            {
                baseGeneric = gmi.GenericMethod.ResolvedMethod;
            }
            return new GenericMethodInstance(baseGeneric, arguments,_host.InternFactory);
        }

        /// <summary>
        /// Retrieves the IFieldDefinition for a field as mapped on a generic type.
        /// </summary>
        private IFieldDefinition GetMappedFieldInfo(IType targetType, IField source)
        {
            var genType = GetSystemType(targetType);
            var fi = GetFieldInfo(source);
            return GetMappedFieldInfo(genType, fi);
        }

        private IFieldDefinition GetMappedFieldInfo(ITypeReference genType, IFieldDefinition fi)
        {
            var result = new Microsoft.Cci.MutableCodeModel.SpecializedFieldDefinition();
            result.Copy(fi, _host.InternFactory);
            result.ContainingTypeDefinition = genType.ResolvedType;
            result.UnspecializedVersion = fi;
            return result;
        }

        /// <summary>
        /// Retrieves the MethodDefinition for a method as mapped on a generic type.
        /// </summary>
        private MethodDefinition GetMappedMethodInfo(IType targetType, IMethod source)
        {
            var mi = GetMethodInfo(source);
            var genType = GetSystemType(targetType);
            var result = new Microsoft.Cci.MutableCodeModel.SpecializedMethodDefinition();
            result.Copy(mi, _host.InternFactory);
            result.ContainingTypeDefinition = genType.ResolvedType;
            result.UnspecializedVersion = mi;
            return result;
        }

        private ITypeReference GetSystemType(Node node)
        {
            return GetSystemType(GetType(node));
        }

        private ITypeReference GetSystemType(IType entity)
        {
            ITypeReference existingType;
            if (_typeCache.TryGetValue(entity, out existingType))
                return existingType;

            ITypeReference type = SystemTypeFrom(entity);
            if (type == null)
                throw new InvalidOperationException(string.Format("Could not find a Type for {0}.", entity));
            _typeCache.Add(entity, type);
            return type;
        }

        private ITypeReference GetFullSystemType(IType entity)
        {
            var result = GetSystemType(entity).ResolvedType;
            if (entity.GenericInfo != null && result.IsGeneric)
            {
                var args = entity.GenericInfo.GenericParameters;
                var cciArgs = args.Select(GetFullSystemType).ToArray();
                return GenericTypeInstance.GetGenericTypeInstance((INamedTypeDefinition)result, cciArgs, _host.InternFactory);
            }
            return result;
        }

        private ITypeReference SystemTypeFrom(IType entity)
        {
            var external = entity as ExternalType;
            if (null != external)
                return GetTypeReference(external.ActualType);

            if (entity.IsArray)
            {
                var arrayType = (TypeSystem.IArrayType)entity;
                var systemType = GetSystemType(arrayType.ElementType);
                var rank = arrayType.Rank;

                var sysArrayType = rank == 1
                    ? (IArrayTypeReference) new VectorTypeReference
                    {
                        InternFactory = _host.InternFactory,
                        ElementType = systemType,
                        Rank = 1
                    }
                    : (IArrayTypeReference) new MatrixTypeReference
                    {
                        InternFactory = _host.InternFactory,
                        ElementType = systemType,
                        Rank = (uint) rank
                    };
                return sysArrayType;
            }

            if (entity.ConstructedInfo != null)
            {
                // Type is a constructed generic type - create it using its definition's system type
                var arguments = Array.ConvertAll(entity.ConstructedInfo.GenericArguments, GetSystemType);
                var sysType = GetSystemType(entity.ConstructedInfo.GenericDefinition);
                return SpecializeType(sysType, arguments);
            }

            if (entity.IsNull())
                return _host.PlatformType.SystemObject;

            if (entity is InternalGenericParameter)
                return (ITypeReference)GetBuilder(((InternalGenericParameter)entity).Node);

            if (entity is AbstractInternalType)
            {
                TypeDefinition typedef = ((AbstractInternalType)entity).TypeDefinition;
                var type = (NamedTypeDefinition)GetBuilder(typedef);

                if (entity.GenericInfo != null && !type.IsGeneric) //hu-oh, early-bound
                    DefineGenericParameters(typedef);

                if (entity.IsPointer && null != type)
                    return PointerType.GetPointerType(type, _host.InternFactory);

                return type;
            }

            return null;
        }

	    private ITypeReference SpecializeType(ITypeReference sysType, ITypeReference[] arguments)
	    {
	        var nested = sysType as ISpecializedNestedTypeDefinition;
            if (nested != null && nested.ContainingTypeDefinition is IGenericTypeInstance)
            {
                var container = (GenericTypeInstance)SpecializeType(nested.ContainingTypeDefinition, arguments).ResolvedType;
                return new Microsoft.Cci.Immutable.SpecializedNestedTypeDefinition(nested.UnspecializedVersion,
                    nested.UnspecializedVersion,
                    container,
                    container,
                    _host.InternFactory);
            }
            var gti = sysType as GenericTypeInstance;
            var baseType = gti != null ? gti.GenericType : (INamedTypeDefinition)sysType.ResolvedType;
            return GenericTypeInstance.GetGenericTypeInstance(baseType.ResolvedType, arguments, _host.InternFactory);
	    }

	    private static void GetNestedTypeAttributes(TypeMember type, NestedTypeDefinition member)
        {
            GetExtendedTypeAttributes(type, member);
            member.Visibility = GetTypeVisibilityAttributes(type);
        }

        private static void GetTypeAttributes(TypeMember type, NamespaceTypeDefinition member)
        {
            GetExtendedTypeAttributes(type, member);
            member.IsPublic = GetTypeVisibilityAttributes(type) == TypeMemberVisibility.Public;
        }

        private static TypeMemberVisibility GetTypeVisibilityAttributes(TypeMember type)
        {
            if (type.IsPublic) return TypeMemberVisibility.Public;
            if (type.IsProtected) return type.IsInternal ? TypeMemberVisibility.FamilyOrAssembly : TypeMemberVisibility.Family;
            if (type.IsInternal) return TypeMemberVisibility.Assembly;
            return TypeMemberVisibility.Private;
        }

        private static void GetExtendedTypeAttributes(TypeMember type, NamedTypeDefinition member)
        {
            switch (type.NodeType)
            {
                case NodeType.ClassDefinition:
                {
                    member.IsClass = true;

                    if (!((ClassDefinition)type).HasDeclaredStaticConstructor)
                    {
                        member.IsBeforeFieldInit = true;
                    }
                    if (type.IsAbstract)
                    {
                        member.IsAbstract = true;
                    }
                    if (type.IsFinal)
                    {
                        member.IsSealed = true;
                    }

                    if (type.IsStatic) //static type is Sealed+Abstract in SRE
                    {
                        member.IsSealed = true;
                        member.IsAbstract = true;
                    }
                    else if (!type.IsTransient)
                        member.IsSerializable = true;

                    if (((IType)type.Entity).IsValueType)
                    {
                        member.IsValueType = true;
                    }
                    break;
                }

                case NodeType.EnumDefinition:
                {
                    member.IsSealed = true;
                    member.IsSerializable = true;
                    break;
                }

                case NodeType.InterfaceDefinition:
                {
                    member.IsInterface = true;
                    member.IsAbstract = true;
                    break;
                }

                case NodeType.Module:
                {
                    member.IsSealed = true;
                    break;
                }
            }
        }

        private static void SetPropertyAttributesFor(Property property, PropertyDefinition builder)
        {
            if (property.ExplicitInfo != null)
            {
                builder.IsSpecialName = true;
                builder.IsRuntimeSpecial = true;
            }
        }

        private void SetMethodAttributesFor(TypeMember member, MethodDefinition builder)
        {
            builder.Visibility = MethodVisibilityAttributesFor(member);

            if (member.IsStatic)
            {
                builder.IsStatic = true;
                if (member.Name.StartsWith("op_"))
                    builder.IsSpecialName = true;
            }
            else if (member.IsAbstract)
            {
                builder.IsAbstract = true;
                builder.IsVirtual = true;
                builder.IsNewSlot = member.IsNew || builder.ContainingTypeDefinition.IsInterface;
            }
            else if (member.IsVirtual || member.IsOverride)
            {
                builder.IsVirtual = true;
                if (member.IsFinal)
                    builder.IsSealed = true;
                if (member.IsNew)
                    builder.IsNewSlot = true;
            }
        }

        private static TypeMemberVisibility MethodVisibilityAttributesFor(TypeMember member)
        {
            if (member.IsPublic)
                return TypeMemberVisibility.Public;
            if (member.IsProtected)
                return member.IsInternal ? TypeMemberVisibility.FamilyOrAssembly : TypeMemberVisibility.Family;
            if (member.IsInternal)
                return TypeMemberVisibility.Assembly;
            return TypeMemberVisibility.Private;
        }

        private void SetPropertyAccessorAttributesFor(TypeMember property, MethodDefinition builder)
        {
            builder.IsSpecialName = true;
            builder.IsHiddenBySignature = true;
            SetMethodAttributesFor(property, builder);
        }

        private void GetMethodAttributes(Method method, MethodDefinition builder)
        {
            builder.IsHiddenBySignature = true;
            if (method.ExplicitInfo != null)
                builder.IsNewSlot = true;
            if (IsPInvoke(method))
            {
                Debug.Assert(method.IsStatic);
                builder.IsPlatformInvoke = true;
            }
            SetMethodAttributesFor(method, builder);
            if (!method.IsStatic)
            {
                var first = method.Parameters.FirstOrDefault();
                if (first != null && first.Name == "self")
                    builder.CallingConvention = CallingConvention.ExplicitThis;
                else builder.CallingConvention = CallingConvention.HasThis;
            }
        }

        private static void SetFieldAttributesFor(Field field, FieldDefinition builder)
        {
            builder.Visibility = FieldVisibilityAttributeFor(field);
            if (field.IsStatic)
                builder.IsStatic = true;
            if (field.IsTransient)
                builder.IsNotSerialized = true;
            if (field.IsFinal)
            {
                var entity = (IField)field.Entity;
                if (entity.IsLiteral)
                    builder.IsCompileTimeConstant = true;
                else
                    builder.IsReadOnly = true;
            }
        }

        private static TypeMemberVisibility FieldVisibilityAttributeFor(Field field)
        {
            if (field.IsProtected)
                return field.IsInternal ? TypeMemberVisibility.FamilyOrAssembly : TypeMemberVisibility.Family;
            if (field.IsPublic)
                return TypeMemberVisibility.Public;
            if (field.IsInternal)
                return TypeMemberVisibility.Assembly;
            return TypeMemberVisibility.Private;
        }

        private static readonly Type IsVolatileType = typeof(System.Runtime.CompilerServices.IsVolatile);

        private static Type[] GetFieldRequiredCustomModifiers(Field field)
        {
            if (field.IsVolatile)
                return new[] { IsVolatileType };
            return Type.EmptyTypes;
        }

        private void DefineEvent(NamedTypeDefinition typeBuilder, Event node)
        {
            var builder = new EventDefinition
            {
                Name = _nameTable.GetNameFor(node.Name),
                Type = GetSystemType(node.Type),
                ContainingTypeDefinition = typeBuilder,
                Adder = DefineEventMethod(typeBuilder, node.Add),
                Remover = DefineEventMethod(typeBuilder, node.Remove),
                Caller = node.Raise != null ? DefineEventMethod(typeBuilder, node.Raise) : null
            };
            builder.Accessors = new System.Collections.Generic.List<IMethodReference>{builder.Adder, builder.Remover};
            if (node.Raise != null)
                builder.Accessors.Add(builder.Caller);
            SetBuilder(node, builder);
            typeBuilder.Events.Add(builder);
        }

        private MethodDefinition DefineEventMethod(NamedTypeDefinition typeBuilder, Method method)
        {
            var result = DefineMethod(typeBuilder, method);
            GetMethodAttributes(method, result);
            result.IsSpecialName = true;
            return result;
        }

	    private ParameterDefinition MakeParameter(ParameterDeclaration value, int index, ISignature parent)
	    {
	        var result = new ParameterDefinition
	        {
	            Type = GetSystemType(value.Type),
                ContainingSignature = parent,
                Index = (ushort)index,
                IsByReference = value.IsByRef,
                Name = _nameTable.GetNameFor(value.Name),                
	        };
	        return result;
	    }

	    private ParameterDefinition NewParameter(ParameterDeclaration value, int index, ISignature parent)
        {
            var result = MakeParameter(value, index, parent);
            SetBuilder(value, result);
            return result;
        }

        private void DefineProperty(NamedTypeDefinition typeBuilder, Property property)
        {
            var name = property.ExplicitInfo != null
                ? property.ExplicitInfo.InterfaceType.Name + "." + property.Name
                : property.Name;

            var builder = new PropertyDefinition
            {
                Name = _nameTable.GetNameFor(name),
                Type = GetSystemType(property.Type),
                ContainingTypeDefinition = typeBuilder,
                Accessors = new System.Collections.Generic.List<IMethodReference>()
            };
            if (!property.IsStatic)
                builder.CallingConvention = CallingConvention.HasThis;
            builder.Parameters = property.Parameters.Select((p, i) => NewParameter(p, i, builder)).Cast<IParameterDefinition>().ToList();
            SetPropertyAttributesFor(property, builder);
            var getter = property.Getter;
            if (getter != null)
            {
                builder.Getter = DefinePropertyAccessor(typeBuilder, property, getter);
                builder.Accessors.Add(builder.Getter);
            }
            var setter = property.Setter;
            if (setter != null)
            {
                builder.Setter = DefinePropertyAccessor(typeBuilder, property, setter);
                builder.Accessors.Add(builder.Setter);
            }
            if (GetEntity(property).IsDuckTyped)
                builder.Attributes = new System.Collections.Generic.List<ICustomAttribute>{CreateDuckTypedCustomAttribute()};

            SetBuilder(property, builder);
            typeBuilder.Properties.Add(builder);
        }

        private MethodDefinition DefinePropertyAccessor(NamedTypeDefinition typeBuilder, Property property, Method accessor)
        {
            if (!accessor.IsVisibilitySet)
                accessor.Visibility = property.Visibility;
            var result = DefineMethod(typeBuilder, accessor);
            SetPropertyAccessorAttributesFor(accessor, result);
            return result;
        }

        private void DefineField(NamedTypeDefinition typeBuilder, Field field)
        {
            var builder = new FieldDefinition
            {
                Name = _nameTable.GetNameFor(field.Name),
                Type = GetSystemType(field),
                ContainingTypeDefinition = typeBuilder,
                CustomModifiers = GetFieldRequiredCustomModifiers(field)
                    .Select(cm => new Microsoft.Cci.Immutable.CustomModifier(false, GetTypeReference(cm)))
                    .Cast<ICustomModifier>().ToList(),
                InternFactory = _host.InternFactory
            };
            SetFieldAttributesFor(field, builder);
            SetBuilder(field, builder);
            typeBuilder.Fields.Add(builder);
        }

        private void DefineParameters(MethodDefinition builder, ParameterDeclarationCollection parameters)
        {
            DefineParameters(parameters, builder);
        }

        private void DefineParameters(ParameterDeclarationCollection parameters, MethodDefinition builder)
        {
            builder.Parameters = new System.Collections.Generic.List<IParameterDefinition>(parameters.Count);
            for (int i = 0; i < parameters.Count; ++i)
            {
                ParameterDefinition paramBuilder = MakeParameter(parameters[i], i, builder);
                if (parameters[i].IsParamArray)
                {
                    SetParamArrayAttribute(paramBuilder);
                    builder.AcceptsExtraArguments = true;
                }
                SetBuilder(parameters[i], paramBuilder);
                builder.Parameters.Add(paramBuilder);
            }
        }

	    private void SetParamArrayAttribute(ParameterDefinition builder)
	    {
            if (builder.Attributes == null)
                builder.Attributes = new System.Collections.Generic.List<ICustomAttribute>();
	        var cons = ConstructorOf<ParamArrayAttribute>();
	        builder.Attributes.Add(new CustomAttribute {Constructor = cons});
	}

        private static void SetImplementationFlagsFor(Method method, MethodDefinition builder)
        {
            if (method.IsRuntime)
            {
                builder.IsRuntimeImplemented = true;
                builder.IsExternal = true;
            }
            else builder.IsCil = true;
        }

        private MethodDefinition DefineMethod(NamedTypeDefinition typeBuilder, Method method)
        {
            var parameters = method.Parameters;

            string name;
            if (method.ExplicitInfo != null)
            {
                name = method.ExplicitInfo.InterfaceType.Name + "." + method.Name;
            }
            else
            {
                name = method.Name;
            }

            var builder = new MethodDefinition
            {
                InternFactory = _host.InternFactory,
                Name = _nameTable.GetNameFor(name),
                ContainingTypeDefinition = typeBuilder
            };

            GetMethodAttributes(method, builder);

            if (method.GenericParameters.Count != 0)
            {
                DefineGenericParameters(builder, method.GenericParameters.ToArray());
            }

            var returnType = GetEntity(method).ReturnType;
            if (IsPInvoke(method) && TypeSystemServices.IsUnknown(returnType))
            {
                returnType = TypeSystemServices.VoidType;
            }
            builder.Type = GetSystemType(returnType);

            SetImplementationFlagsFor(method, builder);

            DefineParameters(builder, parameters);

            SetBuilder(method, builder);
            typeBuilder.Methods.Add(builder);

            var methodEntity = GetEntity(method);
            if (methodEntity.IsDuckTyped)
            {
                if (builder.Attributes == null)
                    builder.Attributes = new System.Collections.Generic.List<ICustomAttribute>();
                builder.Attributes.Add(CreateDuckTypedCustomAttribute());
            }
            return builder;
        }

        private void DefineGenericParameters(TypeDefinition typeDefinition)
        {
            if (typeDefinition is EnumDefinition)
                return;

            var type = GetTypeBuilder(typeDefinition);
            if (type.IsGeneric)
                return; //early-bound, do not redefine generic parameters again

            if (typeDefinition.GenericParameters.Count > 0)
            {
                var nested = type as NestedTypeDefinition;
                if (nested != null)
                    DefineNestedGenericParameters(type, nested.ContainingTypeDefinition, typeDefinition.GenericParameters.ToArray());
                else DefineGenericParameters(type, typeDefinition.GenericParameters.ToArray());
            }
        }

        private void DefineNestedGenericParameters(NamedTypeDefinition builder, ITypeDefinition parent, GenericParameterDeclaration[] parameters)
        {
            var parentParams = GetAllGenericParameters((INamedTypeDefinition)parent).ToDictionary(p => p.Name.Value);
            var ownParameters = parameters.Where(p => !parentParams.ContainsKey(p.Name)).ToArray();
            var builders = ownParameters
                .Select((gpd, i) => new GenericTypeParameter
                {
                    Name = _nameTable.GetNameFor(gpd.Name),
                    DefiningType = builder,
                    Index = (ushort)i,
                    InternFactory = _host.InternFactory
                })
                .ToArray();
            if (builders.Length < parameters.Length)
                foreach (var param in parameters)
                {
                    IGenericTypeParameter decl;
                    if (parentParams.TryGetValue(param.Name, out decl))
                        SetBuilder(param, decl);
                }
            if (builders.Length > 0)
            {
                builder.GenericParameters = new System.Collections.Generic.List<IGenericTypeParameter>(
                    builders.Where(p => !parentParams.ContainsKey(p.Name.Value)));
                DefineGenericParameters(builders, ownParameters);
            }
        }

        /// <summary>
        /// Defines the generic parameters of an internal generic type.
        /// </summary>
        private void DefineGenericParameters(NamedTypeDefinition builder, GenericParameterDeclaration[] parameters)
        {
            var builders = parameters.Select((gpd, i) => new GenericTypeParameter
            {
                Name = _nameTable.GetNameFor(gpd.Name),
                DefiningType = builder,
                Index = (ushort) i,
                InternFactory = _host.InternFactory
            }).ToArray();
            builder.GenericParameters = new System.Collections.Generic.List<IGenericTypeParameter>(builders);

            DefineGenericParameters(builders, parameters);
        }

        /// <summary>
        /// Defines the generic parameters of an internal generic method.
        /// </summary>
        private void DefineGenericParameters(MethodDefinition builder, GenericParameterDeclaration[] parameters)
        {
            var builders = parameters.Select(delegate(GenericParameterDeclaration gpd, int i)
                {
                    var result = new GenericMethodParameter
                    {
                        Name = _nameTable.GetNameFor(gpd.Name),
                        DefiningMethod = builder,
                        Index = (ushort) i,
                        InternFactory = _host.InternFactory,
                        Constraints = gpd.BaseTypes.Select(GetSystemType).ToList()
                    };
                    SetBuilder(gpd, result);
                    return result;
                }
            ).ToArray();
            builder.GenericParameters = new System.Collections.Generic.List<IGenericMethodParameter>(builders);

            DefineGenericParameters(builders, parameters);
            builder.CallingConvention |= CallingConvention.Generic;
        }

        private void DefineGenericParameters(GenericParameter[] builders, GenericParameterDeclaration[] declarations)
        {
            for (int i = 0; i < builders.Length; i++)
            {
                SetBuilder(declarations[i], builders[i]);
                DefineGenericParameter(((InternalGenericParameter)declarations[i].Entity), builders[i]);
            }
        }

        private void DefineGenericParameter(InternalGenericParameter parameter, GenericParameter builder)
        {
            if (parameter.BaseType != TypeSystemServices.ObjectType || parameter.GetInterfaces().Length > 0)
            {
                builder.Constraints = new System.Collections.Generic.List<ITypeReference>();
                // Set base type constraint
                if (parameter.BaseType != TypeSystemServices.ObjectType)
                {
                    builder.Constraints.Add(GetFullSystemType(parameter.BaseType))
                        ;
                }
                builder.Constraints.AddRange(parameter.GetInterfaces().Select(GetFullSystemType));
            }

            // Set special attributes
            if (parameter.IsClass)
                builder.MustBeReferenceType = true;
            if (parameter.IsValueType)
                builder.MustBeValueType = true;
            if (parameter.MustHaveDefaultConstructor)
                builder.MustHaveDefaultConstructor = true;
        }

        private CustomAttribute CreateDuckTypedCustomAttribute()
        {
            var cons = ConstructorOf<DuckTypedAttribute>();
            return new CustomAttribute{Constructor = cons};
        }

        private void DefineConstructor(NamedTypeDefinition typeBuilder, Method constructor)
        {
            var builder = new MethodDefinition
            {
                InternFactory = _host.InternFactory,
                IsRuntimeSpecial = true,
                IsSpecialName = true,
                IsStatic = constructor.IsStatic,
                Name = _nameTable.GetNameFor(constructor.IsStatic ? ".cctor" : ".ctor"),
                CallingConvention = constructor.IsStatic ? CallingConvention.Default : CallingConvention.HasThis,
                ContainingTypeDefinition = typeBuilder,
                Type = _host.PlatformType.SystemVoid
            };
            GetMethodAttributes(constructor, builder);

            SetImplementationFlagsFor(constructor, builder);
            DefineParameters(builder, constructor.Parameters);

            SetBuilder(constructor, builder);
            typeBuilder.Methods.Add(builder);
        }

        private static bool IsEnumDefinition(TypeMember type)
        {
            return type.NodeType == NodeType.EnumDefinition;
        }

        private void DefineType(TypeDefinition typeDefinition)
        {
            SetBuilder(typeDefinition, CreateTypeBuilder(typeDefinition));
        }

	    private static bool IsValueType(TypeMember type)
        {
            var entity = type.Entity as IType;
            return null != entity && entity.IsValueType;
        }

        private InternalLocal GetInternalLocal(Node local)
        {
            return (InternalLocal)GetEntity(local);
        }

        private readonly Dictionary<string, UnitNamespace> _namespaceMap = new Dictionary<string, UnitNamespace>();

        private UnitNamespace EnsureNamespace(string name)
	    {
            UnitNamespace result;
	        if (!_namespaceMap.TryGetValue(name, out result))
	        {
	            var subLength = name.LastIndexOf(".", StringComparison.InvariantCulture);
	            var parentName = subLength > 0 ? name.Substring(0, subLength) : "";
                var myName = name.Substring(subLength + 1);
	            var parentNamespace = EnsureNamespace(parentName);
	            result = new NestedUnitNamespace
	            {
	                ContainingUnitNamespace = parentNamespace,
                    Name = _nameTable.GetNameFor(myName)
	            };
                parentNamespace.Members.Add((NestedUnitNamespace)result);
                _namespaceMap.Add(name, result);
	        }
	        return result;
	    }

	    private object CreateTypeBuilder(TypeDefinition type)
        {
            ITypeReference baseType = null;
            if (IsEnumDefinition(type))
            {
                baseType = _host.PlatformType.SystemEnum;
            }
            else if (IsValueType(type))
            {
                baseType = _host.PlatformType.SystemValueType;
            }

            NamedTypeDefinition typeBuilder = null;
            var enclosingType = type.ParentNode as ClassDefinition;
            var enumDef = type as EnumDefinition;
            var enclosingNamespace = type.EnclosingNamespace != null ? EnsureNamespace(type.EnclosingNamespace.Name) : (UnitNamespace)_asmBuilder.UnitNamespaceRoot;

            if (null == enclosingType)
            {
                typeBuilder = new NamespaceTypeDefinition
                {
                    ContainingUnitNamespace = enclosingNamespace,
                    InternFactory = _host.InternFactory,
                    Name = _nameTable.GetNameFor(type.Name),
                    Methods = new System.Collections.Generic.List<IMethodDefinition>(),
                    Fields = new System.Collections.Generic.List<IFieldDefinition>(),
                    Properties = new System.Collections.Generic.List<IPropertyDefinition>(),
                    NestedTypes = new System.Collections.Generic.List<INestedTypeDefinition>(),
                    Events = new System.Collections.Generic.List<IEventDefinition>()
                };
                enclosingNamespace.Members.Add((NamespaceTypeDefinition)typeBuilder);
                _asmBuilder.AllTypes.Add(typeBuilder);
                if (baseType != null)
                    typeBuilder.BaseClasses = new System.Collections.Generic.List<ITypeReference>{baseType};
                GetTypeAttributes(type, (NamespaceTypeDefinition)typeBuilder);
            }
            else
            {
                var enclosingTypeBuilder = GetTypeBuilder(enclosingType);
                typeBuilder = new NestedTypeDefinition
                {
                    ContainingTypeDefinition = enclosingTypeBuilder,
                    InternFactory = _host.InternFactory,
                    Name = _nameTable.GetNameFor(type.Name),
                    NestedTypes = new System.Collections.Generic.List<INestedTypeDefinition>(),
                    Fields = new System.Collections.Generic.List<IFieldDefinition>(),
                    Methods = new System.Collections.Generic.List<IMethodDefinition>(),
                    Properties = new System.Collections.Generic.List<IPropertyDefinition>()
                };
                enclosingTypeBuilder.NestedTypes.Add((NestedTypeDefinition)typeBuilder);
                _asmBuilder.AllTypes.Add(typeBuilder);
                if (baseType != null)
                    typeBuilder.BaseClasses = new System.Collections.Generic.List<ITypeReference> { baseType };
                GetNestedTypeAttributes(type, (NestedTypeDefinition)typeBuilder);
            }

            if (IsEnumDefinition(type))
            {
                // Mono cant construct enum array types unless
                // the fields is already defined
                typeBuilder.Fields.Add(new FieldDefinition
                {
                    InternFactory = _host.InternFactory,
                    Type = GetTypeReference(GetEnumUnderlyingType(enumDef)),
                    Name = _nameTable.GetNameFor("value__"),
                    IsSpecialName = true,
                    IsRuntimeSpecial = true,
                    Visibility = TypeMemberVisibility.Public,
                    ContainingTypeDefinition = typeBuilder
                });
            }
            else if (IsValueType(type))
                typeBuilder.Layout = LayoutKind.Sequential;

            var layout = type.Attributes.FirstOrDefault(a => a.Entity is ExternalConstructor && 
                ((ExternalConstructor)a.Entity).ConstructorInfo.DeclaringType == typeof(System.Runtime.InteropServices.StructLayoutAttribute));
            if (layout != null)
                ApplyLayoutAttribute(layout, typeBuilder);

            return typeBuilder;
        }

	    private void ApplyLayoutAttribute(Attribute layout, NamedTypeDefinition typeBuilder)
	    {
            var layoutKind = layout.Arguments.Single();
            if (layoutKind.NodeType == NodeType.MemberReferenceExpression)
                typeBuilder.Layout = (LayoutKind)Enum.Parse(typeof(LayoutKind), ((MemberReferenceExpression)layoutKind).Name);
            else typeBuilder.Layout = (LayoutKind) int.Parse(layoutKind.ToString());
            foreach (var namedArg in layout.NamedArguments)
            {
                switch (namedArg.First.ToString())
                {
                    case "Size":
                        typeBuilder.SizeOf = uint.Parse(namedArg.Second.ToString());
                        break;
                    default:
                        CompilerContext.Current.Warnings.Add(new CompilerWarning(namedArg.LexicalInfo, "Attribute " + namedArg.First + " is not supported"));
                        break;
                }
            }
	    }

	    private void EmitBaseTypesAndAttributes(TypeDefinition typeDefinition, NamedTypeDefinition typeBuilder)
        {
            foreach (Ast.TypeReference baseType in typeDefinition.BaseTypes)
            {
                var type = GetSystemType(baseType);

                // For some reason you can't call IsClass on constructed types created at compile time,
                // so we'll ask the generic definition instead
                if ((type.ResolvedType.IsGeneric && ((IGenericTypeInstance)type).GenericType.ResolvedType.IsClass) || (type.ResolvedType.IsClass))
                {
                    typeBuilder.BaseClasses = new System.Collections.Generic.List<ITypeReference>{type};
                }
                else
                {
                    if (typeBuilder.Interfaces == null)
                        typeBuilder.Interfaces = new System.Collections.Generic.List<ITypeReference>();
                    typeBuilder.Interfaces.Add(type);
                }
            }
        }

        private static void NotImplemented(string feature)
        {
            throw new NotImplementedException(feature);
        }

        private CustomAttribute GetCustomAttributeBuilder(Attribute node)
        {
            var constructor = (IConstructor)GetEntity(node);
            var constructorInfo = GetConstructorInfo(constructor);
            var constructorArgs = ArgumentsForAttributeConstructor(constructor, node.Arguments);

            var namedArgs = node.NamedArguments;
            if (namedArgs.Count > 0)
            {
                var namedValues = GetNamedValues(namedArgs);
                return new CustomAttribute
                {
                    Constructor = constructorInfo,
                    Arguments = constructorArgs,
                    NamedArguments = namedValues
                };
            }
            return new CustomAttribute
            {
                Constructor = constructorInfo,
                Arguments = constructorArgs,
            };
        }

        private System.Collections.Generic.List<IMetadataNamedArgument> GetNamedValues(ExpressionPairCollection values)
        {
            var result = new System.Collections.Generic.List<IMetadataNamedArgument>();
            foreach (var pair in values)
            {
                ITypedEntity entity = (ITypedEntity)GetEntity(pair.First);
                var value = GetValue(entity.Type, pair.Second);
                var argType = value.Type;
                if (EntityType.Property == entity.EntityType)
                {
                    result.Add(new MetadataNamedArgument
                    {
                        ArgumentName = _nameTable.GetNameFor(((IProperty) entity).Name),
                        ArgumentValue = value,
                        Type = argType
                    });
                }
                else
                {
                    result.Add(new MetadataNamedArgument
                    {
                        ArgumentName = _nameTable.GetNameFor(((IField)entity).Name),
                        ArgumentValue = value,
                        IsField = true,
                        Type = argType
                    });
                }
            }

            return result;
        }

        private System.Collections.Generic.List<IMetadataExpression> ArgumentsForAttributeConstructor(IConstructor ctor, ExpressionCollection args)
        {
            var varargs = ctor.AcceptVarArgs;
            var parameters = ctor.GetParameters();
            var result = new System.Collections.Generic.List<IMetadataExpression>(parameters.Length);
            var lastIndex = parameters.Length - 1;
            var fixedParameters = varargs ? parameters.Take(lastIndex) : parameters;

            var i = 0;
            foreach (var parameter in fixedParameters)
            {
                result.Add(GetValue(parameter.Type, args[i]));
                ++i;
            }

            if (varargs)
            {
                var varArgType = parameters[lastIndex].Type.ElementType;
                result.Add(new MetadataCreateArray
                {
                    Initializers =
                        args.Skip(lastIndex).Select(e => GetValue(varArgType, e)).ToList()
                });
            }
            return result;
        }

        private IMetadataExpression GetValue(IType expectedType, Expression expression)
        {
            switch (expression.NodeType)
            {
                case NodeType.NullLiteralExpression:
                    return new MetadataConstant();

                case NodeType.StringLiteralExpression:
                    return new MetadataConstant{ Type = GetTypeReference<string>(), Value = ((StringLiteralExpression) expression).Value };

                case NodeType.CharLiteralExpression:
                    return new MetadataConstant { Type = GetTypeReference<char>(), Value = ((CharLiteralExpression)expression).Value[0] };

                case NodeType.BoolLiteralExpression:
                    return new MetadataConstant { Type = GetTypeReference<bool>(), Value = ((BoolLiteralExpression)expression).Value };

                case NodeType.IntegerLiteralExpression:
                    var ile = (IntegerLiteralExpression)expression;
                    object intValue;
                    if (ile.IsLong)
                        intValue = ile.Value;
                    else
                        intValue = (int) ile.Value;
                    return ConvertValue(expectedType, intValue);

                case NodeType.DoubleLiteralExpression:
                    var dle = (DoubleLiteralExpression) expression;
                    object floatValue;
                    if (dle.IsSingle)
                        floatValue = (float)dle.Value;
                    else floatValue = dle.Value;
                    return ConvertValue(expectedType, floatValue);

                case NodeType.TypeofExpression:
                    var typeofType = GetSystemType(((TypeofExpression)expression).Type);
                    return new MetadataTypeOf{TypeToGet = typeofType, Type = _host.PlatformType.SystemType};

                case NodeType.CastExpression:
                    return GetValue(expectedType, ((CastExpression)expression).Target);

                default:
                    return GetComplexExpressionValue(expectedType, expression);
            }
        }

        private IMetadataExpression GetComplexExpressionValue(IType expectedType, Expression expression)
        {
            IEntity tag = GetEntity(expression);
            if (tag.EntityType == EntityType.Type)
                return new MetadataTypeOf{TypeToGet = GetSystemType(expression)};

            if (EntityType.Field == tag.EntityType)
            {
                var field = (IField)tag;
                if (field.IsLiteral)
                {
                    //Scenario:
                    //IF:
                    //SomeType.StaticReference = "hamsandwich"
                    //[RandomAttribute(SomeType.StaticReferenece)]
                    //THEN:
                    //field.StaticValue != "hamsandwich"
                    //field.StaticValue == SomeType.StaticReference
                    //SO:
                    //If field.StaticValue is an AST Expression, call GetValue() on it
                    if (field.StaticValue is Expression)
                        return GetValue(expectedType, field.StaticValue as Expression);
                    return new MetadataConstant{Type = GetTypeReference(field.StaticValue.GetType()), Value = field.StaticValue};
                }
            }

            NotImplemented(expression, "Expression value: " + expression);
            return null;
        }

        private IMetadataConstant ConvertValue(IType expectedType, object value)
        {
            ITypeReference newType;
            if (expectedType.IsEnum)
            {
                var enumType = GetEnumUnderlyingType(expectedType);
                newType = GetTypeReference(enumType);
                value = Convert.ChangeType(value, enumType);
            }
            else newType = GetSystemType(expectedType);
            return new MetadataConstant { Type = newType, Value = value };
        }

        private static Type GetEnumUnderlyingType(EnumDefinition node)
        {
            return ((InternalEnum)node.Entity).UnderlyingType;
        }

        private static Type GetEnumUnderlyingType(IType enumType)
        {
            return enumType is IInternalEntity
                ? ((InternalEnum)enumType).UnderlyingType
                : Enum.GetUnderlyingType(((ExternalType)enumType).ActualType);
        }

        private void DefineTypeMembers(TypeDefinition typeDefinition)
        {
            if (IsEnumDefinition(typeDefinition))
            {
                return;
            }
            NamedTypeDefinition typeBuilder = GetTypeBuilder(typeDefinition);
            TypeMemberCollection members = typeDefinition.Members;
            foreach (TypeMember member in members)
            {
                switch (member.NodeType)
                {
                    case NodeType.Method:
                        {
                            DefineMethod(typeBuilder, (Method)member);
                            break;
                        }

                    case NodeType.Constructor:
                        {
                            DefineConstructor(typeBuilder, (Constructor)member);
                            break;
                        }

                    case NodeType.Field:
                        {
                            DefineField(typeBuilder, (Field)member);
                            break;
                        }

                    case NodeType.Property:
                        {
                            DefineProperty(typeBuilder, (Property)member);
                            break;
                        }

                    case NodeType.Event:
                        {
                            DefineEvent(typeBuilder, (Event)member);
                            break;
                        }
                }
            }
        }

        string GetAssemblySimpleName(string fname)
        {
            return Path.GetFileNameWithoutExtension(fname);
        }

        private string GetTargetDirectory(string fname)
        {
            return Permissions.WithDiscoveryPermission(() => Path.GetDirectoryName(Path.GetFullPath(fname)));
        }

        private string BuildOutputAssemblyName()
        {
            string configuredOutputAssembly = Parameters.OutputAssembly;
            if (!string.IsNullOrEmpty(configuredOutputAssembly))
                return TryToGetFullPath(configuredOutputAssembly);

            string outputAssembly = CompileUnit.Modules[0].Name;
            if (!HasDllOrExeExtension(outputAssembly))
            {
                if (Parameters.OutputType == CompilerOutputType.Library)
                    outputAssembly += ".dll";
                else
                    outputAssembly += ".exe";
            }
            return TryToGetFullPath(outputAssembly);
        }

        private static string TryToGetFullPath(string path)
        {
            return Permissions.WithDiscoveryPermission(() => Path.GetFullPath(path)) ?? path;
        }

        private static bool HasDllOrExeExtension(string fname)
        {
            var extension = Path.GetExtension(fname);
            switch (extension.ToLower())
            {
                case ".dll":
                case ".exe":
                    return true;
                default:
                    return false;
            }
        }

        private void DefineResources()
        {
            foreach (ICompilerResource resource in Parameters.Resources)
                resource.WriteResource(_sreResourceService);
        }

        private SreResourceService _sreResourceService;

	    private sealed class SreResourceService : IResourceService
        {
            private readonly Assembly _asmBuilder;
            private readonly Microsoft.Cci.MutableCodeModel.Module _moduleBuilder;
            private readonly INameTable _nameTable;

            public SreResourceService(Assembly asmBuilder, Microsoft.Cci.MutableCodeModel.Module modBuilder, INameTable nameTable)
            {
                _asmBuilder = asmBuilder;
                _moduleBuilder = modBuilder;
                _nameTable = nameTable;
            }

            public bool EmbedFile(string resourceName, string fname)
            {
                _asmBuilder.Resources.Add(new Resource
                {
                    IsPublic = true,
                    Name = _nameTable.GetNameFor(resourceName),
                    IsInExternalFile = true,
                    ExternalFile =
                        new FileReference {ContainingAssembly = _asmBuilder, FileName = _nameTable.GetNameFor(fname)},
                    DefiningAssembly = _asmBuilder
                });
                return true;
            }

            public IResourceWriter DefineResource(string resourceName, string resourceDescription)
            {
                return new BooResourceWriter(_asmBuilder, _nameTable.GetNameFor(resourceName));
            }
        }

        private void SetUpAssembly()
        {
            var outputFile = BuildOutputAssemblyName();
            var baseFilename = Path.GetFileName(outputFile);
            var asmName = CreateAssemblyName(outputFile);
            //var assemblyBuilderAccess = GetAssemblyBuilderAccess();
            var rootUnitNamespace = new RootUnitNamespace();
            if (_host == null)
                _host = new PeReader.DefaultHost();
            _nameTable = _host.NameTable;
            _coreAssembly = _host.LoadAssembly(_host.CoreAssemblySymbolicIdentity);
            _asmBuilder = new Assembly
            {
                Name = _nameTable.GetNameFor(Path.GetFileNameWithoutExtension(baseFilename)),
                ModuleName = _nameTable.GetNameFor(baseFilename),
                Location = outputFile,
                Kind = GetModuleKind(),
                PlatformType = _host.PlatformType,
                TargetRuntimeVersion = _coreAssembly.TargetRuntimeVersion,
                AssemblyAttributes = new System.Collections.Generic.List<ICustomAttribute>(),
                ModuleAttributes = new System.Collections.Generic.List<ICustomAttribute>(),
                UnitNamespaceRoot = rootUnitNamespace,
                AssemblyReferences = new System.Collections.Generic.List<IAssemblyReference>(Parameters.References
                    .Cast<TypeSystem.Reflection.IAssemblyReference>()
                    .Select(ar => MakeAssemblyReference(ar.Assembly))),
                TrackDebugData = true,
                Version = asmName.Version,
                RequiresStartupStub = _host.PointerSize == 4,
            };
            foreach (var pair in Context.CodegenMap.Where(p => !_asmCache.ContainsKey(p.Key)))
                _asmCache.Add(pair.Key, pair.Value);
            rootUnitNamespace.Unit = _asmBuilder;
            _namespaceMap.Add("", rootUnitNamespace);
            
            _moduleClass = new NamespaceTypeDefinition
            {
                ContainingUnitNamespace = rootUnitNamespace,
                InternFactory = _host.InternFactory,
                IsClass = true,
                Name = _nameTable.GetNameFor("<Module>"),
                Methods = new System.Collections.Generic.List<IMethodDefinition>(),
                Fields = new System.Collections.Generic.List<IFieldDefinition>()
            };
            _asmBuilder.AllTypes.Add(_moduleClass);

            SetupBuiltins();

            if (Parameters.Debug)
            {
                // ikvm tip:  Set DebuggableAttribute to assembly before
                // creating the module, to make sure Visual Studio (Whidbey)
                // picks up the attribute when debugging dynamically generated code.
                _asmBuilder.AssemblyAttributes.Add(CreateDebuggableAttribute());
            }

            _asmBuilder.AssemblyAttributes.Add(CreateRuntimeCompatibilityAttribute());
            _moduleBuilder = _asmBuilder;

            if (Parameters.Unsafe)
                _moduleBuilder.ModuleAttributes.Add(CreateUnverifiableCodeAttribute());

            _sreResourceService = new SreResourceService(_asmBuilder, _moduleBuilder, _nameTable);
            ContextAnnotations.SetAssemblyBuilderCci(Context, _asmBuilder);
            ContextAnnotations.SetCciHost(Context, _host);

            Context.GeneratedAssemblyFileName = outputFile;
        }

	    private IAssemblyReference MakeAssemblyReference(System.Reflection.Assembly value)
	    {
	        var name = value.GetName();
	        var ident = new AssemblyIdentity(_nameTable.GetNameFor(name.Name), name.CultureInfo.Name, name.Version,
	            name.GetPublicKeyToken(), name.CodeBase);
	        return new Microsoft.Cci.Immutable.AssemblyReference(_host, ident);
	    }

	    private ModuleKind GetModuleKind()
	    {
	        switch (Parameters.OutputType)
	        {
	            case CompilerOutputType.Auto:
                    return ContextAnnotations.GetEntryPoint(Context) != null ? ModuleKind.ConsoleApplication : ModuleKind.DynamicallyLinkedLibrary;
	            case CompilerOutputType.Library:
	                return ModuleKind.DynamicallyLinkedLibrary;
	            case CompilerOutputType.ConsoleApplication:
	                return ModuleKind.ConsoleApplication;
	            case CompilerOutputType.WindowsApplication:
	                return ModuleKind.WindowsApplication;
	            default:
	                throw new ArgumentOutOfRangeException();
	        }
	    }

        /*
	    AssemblyBuilderAccess GetAssemblyBuilderAccess()
        {
            if (Parameters.GenerateCollectible)
            {
#if !NET_40_OR_GREATER

                Context.Warnings.Add(CompilerWarningFactory.CustomWarning("Collectible Assemblies are available only on .NET Framework 4.0 or later (https://msdn.microsoft.com/en-us/library/dd554932(v=vs.100).aspx)"));
                return Parameters.GenerateInMemory ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Save;
#else
				return Parameters.GenerateInMemory ? AssemblyBuilderAccess.RunAndCollect : AssemblyBuilderAccess.Save;
#endif
            }
            else
            {
                return Parameters.GenerateInMemory ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Save;
            }

        }
        */

	    private System.Reflection.AssemblyName CreateAssemblyName(string outputFile)
        {
            var assemblyName = new System.Reflection.AssemblyName
            {
                Name = GetAssemblySimpleName(outputFile),
                Version = GetAssemblyVersion()
            };
            if (Parameters.DelaySign)
                assemblyName.SetPublicKey(GetAssemblyKeyPair(outputFile).PublicKey);
            else
                assemblyName.KeyPair = GetAssemblyKeyPair(outputFile);
            return assemblyName;
        }

        private System.Reflection.StrongNameKeyPair GetAssemblyKeyPair(string outputFile)
        {
            var attribute = GetAssemblyAttribute("System.Reflection.AssemblyKeyNameAttribute");
            if (Parameters.KeyContainer != null)
            {
                if (attribute != null)
                    Warnings.Add(CompilerWarningFactory.HaveBothKeyNameAndAttribute(attribute));
                if (Parameters.KeyContainer.Length != 0)
                    return new System.Reflection.StrongNameKeyPair(Parameters.KeyContainer);
            }
            else if (attribute != null)
            {
                var asmName = ((StringLiteralExpression)attribute.Arguments[0]).Value;
                if (asmName.Length != 0) //ignore empty AssemblyKeyName values, like C# does
                    return new System.Reflection.StrongNameKeyPair(asmName);
            }

            string fname = null;
            string srcFile = null;
            attribute = GetAssemblyAttribute("System.Reflection.AssemblyKeyFileAttribute");

            if (Parameters.KeyFile != null)
            {
                fname = Parameters.KeyFile;
                if (attribute != null)
                    Warnings.Add(CompilerWarningFactory.HaveBothKeyFileAndAttribute(attribute));
            }
            else if (attribute != null)
            {
                fname = ((StringLiteralExpression)attribute.Arguments[0]).Value;
                if (attribute.LexicalInfo != null)
                    srcFile = attribute.LexicalInfo.FileName;
            }

            if (!string.IsNullOrEmpty(fname))
            {
                if (!Path.IsPathRooted(fname))
                    fname = ResolveRelative(outputFile, srcFile, fname);
                using (FileStream stream = File.OpenRead(fname))
                {
                    //Parameters.DelaySign is ignored.
                    return new System.Reflection.StrongNameKeyPair(stream);
                }
            }
            return null;
        }

	    private string ResolveRelative(string targetFile, string srcFile, string relativeFile)
        {
            //relative to current directory:
            var fname = Path.GetFullPath(relativeFile);
            if (File.Exists(fname))
                return fname;

            //relative to source file:
            if (srcFile != null)
            {
                fname = ResolveRelativePath(srcFile, relativeFile);
                if (File.Exists(fname))
                    return fname;
            }

            //relative to output assembly:
            if (targetFile != null)
                return ResolveRelativePath(targetFile, relativeFile);

            return fname;
        }

        private static string ResolveRelativePath(string srcFile, string relativeFile)
        {
            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(srcFile), relativeFile));
        }

	    private Version GetAssemblyVersion()
        {
            var version = GetAssemblyAttributeValue("System.Reflection.AssemblyVersionAttribute");
            if (version == null)
                return new Version();

            /* 1.0.* -- BUILD -- based on days since January 1, 2000
			 * 1.0.0.* -- REVISION -- based on seconds since midnight, January 1, 2000, divided by 2			 *
			 */
            string[] sliced = version.Split('.');
            if (sliced.Length > 2)
            {
                var baseTime = new DateTime(2000, 1, 1);
                var mark = DateTime.Now - baseTime;
                if (sliced[2].StartsWith("*"))
                    sliced[2] = Math.Round(mark.TotalDays).ToString();
                if (sliced.Length > 3)
                    if (sliced[3].StartsWith("*"))
                        sliced[3] = Math.Round(mark.TotalSeconds).ToString();
                version = string.Join(".", sliced);
            }
            return new Version(version);
        }

	    private string GetAssemblyAttributeValue(string name)
        {
            var attribute = GetAssemblyAttribute(name);
            return null != attribute ? ((StringLiteralExpression)attribute.Arguments[0]).Value : null;
        }

	    private Attribute GetAssemblyAttribute(string name)
        {
            return _assemblyAttributes.Get(name).FirstOrDefault();
        }

        protected override IType GetExpressionType(Expression node)
        {
            var type = base.GetExpressionType(node);
            if (TypeSystemServices.IsUnknown(type)) throw CompilerErrorFactory.InvalidNode(node);
            return type;
        }

        private IMethodDefinition StringFormat
        {
            get {
                return _stringFormat ?? (_stringFormat = MethodOf<string>("Format", typeof(string), typeof(object)));
            }
        }

	    private static IMethodDefinition _stringFormat;

        private readonly Dictionary<ITypeReference, IMethodDefinition> _nullableHasValue = new Dictionary<ITypeReference, IMethodDefinition>();

        private IMethodDefinition GetNullableHasValue(ITypeReference type)
        {
            IMethodDefinition method;
            if (_nullableHasValue.TryGetValue(type, out method))
                return method;
            var specializedType = GenericTypeInstance.GetGenericTypeInstance(_nullableType, new []{type}, _host.InternFactory);
            method = TypeHelper.GetProperty(specializedType, _nameTable.GetNameFor("HasValue")).Getter.ResolvedMethod;
            _nullableHasValue.Add(type, method);
            return method;
        }
    }
}
