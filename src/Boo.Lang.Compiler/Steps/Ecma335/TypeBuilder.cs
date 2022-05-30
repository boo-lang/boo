using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Internal;
using TypeDefinition = Boo.Lang.Compiler.Ast.TypeDefinition;

namespace Boo.Lang.Compiler.Steps.Ecma335
{
	[DebuggerDisplay("{_type}")]
    internal class TypeBuilder : IBuilder
    {
        private readonly string _name;
		private IType _parent;
		private TypeAttributes _attrs;
        private readonly TypeDefinition _type;
        private readonly BlobBuilder _ilBuilder;
        private readonly EntityHandle _handle;
        private readonly bool _isDebug;
        private readonly List<TypeBuilder> _nestedTypes = new();
		private readonly List<FieldBuilder> _fields = new();
		private readonly List<MethodBuilder> _methods = new();
		private readonly List<PropertyBuilder> _properties = new();
		private readonly List<EventBuilder> _events = new();
		private readonly HashSet<IType> _interfaceImplementations = new();
		private readonly List<KeyValuePair<EntityHandle, EntityHandle>> _explicitImpls = new();
		private readonly TypeSystemBridge _typeSystem;

		private GenericTypeParameterBuilder[] _genParams;

        public TypeBuilder(TypeDefinition type, BlobBuilder ilBuilder, TypeSystemBridge typeSystem, bool isDebug)
			: this(type, ilBuilder, GetTypeAttributes(type), typeSystem, isDebug)
		{ }

		private TypeBuilder(TypeDefinition type, BlobBuilder ilBuilder, TypeAttributes attrs, TypeSystemBridge typeSystem, bool isDebug)
		{
			_name = AnnotateGenericTypeName(type, type.Name);
			_attrs = attrs;
			_type = type;
			_ilBuilder = ilBuilder;
			_typeSystem = typeSystem;
			_handle = typeSystem.ReserveType((IType)type.Entity);
			_isDebug = isDebug;
		}

		public TypeBuilder DefineNestedType(TypeDefinition type)
		{
			var result = new TypeBuilder(type, _ilBuilder, GetNestedTypeAttributes(type), _typeSystem, _isDebug);
			_nestedTypes.Add(result);
			return result;
		}

		IEntity IBuilder.Entity => _type.Entity;

		public EntityHandle Handle => _handle;

		public bool IsGenericType => ((IType)_type.Entity).ConstructedInfo != null;

        public FieldBuilder DefineField(IField field, IType type, FieldAttributes attributes, object value = null)
		{
			// check for modreqs: see below
			var result = new FieldBuilder(field, type, attributes, _typeSystem, value);
			_fields.Add(result);
			return result;
		}

		public MethodBuilder DefineMethod(InternalMethod method, MethodAttributes attrs)
		{
			var result = new MethodBuilder(method, new MethodBodyStreamEncoder(_ilBuilder), attrs, _isDebug, _typeSystem);
			_methods.Add(result);
			return result;
		}

		public EventBuilder DefineEvent(IEvent ev)
		{
			var result = new EventBuilder(ev, _typeSystem);
			_events.Add(result);
			return result;
		}

		public PropertyBuilder DefineProperty(IProperty prop, PropertyAttributes attrs)
		{
			var result = new PropertyBuilder(prop, attrs, _typeSystem);
			_properties.Add(result);
			return result;
		}

		private static string AnnotateGenericTypeName(TypeDefinition typeDef, string name) =>
            typeDef.HasGenericParameters
                ? $"{name}`{typeDef.GenericParameters.Count}"
                : name;

        static TypeAttributes GetNestedTypeAttributes(TypeMember type) =>
            GetExtendedTypeAttributes(GetNestedTypeAccessibility(type), type);

        static TypeAttributes GetNestedTypeAccessibility(TypeMember type)
        {
            if (type.IsPublic) return TypeAttributes.NestedPublic;
            if (type.IsInternal) return TypeAttributes.NestedAssembly;
            return TypeAttributes.NestedPrivate;
        }

        private static TypeAttributes GetTypeAttributes(TypeMember type) =>
            GetExtendedTypeAttributes(GetTypeVisibilityAttributes(type), type);

        private static TypeAttributes GetTypeVisibilityAttributes(TypeMember type) =>
            type.IsPublic ? TypeAttributes.Public : TypeAttributes.NotPublic;

        static TypeAttributes GetExtendedTypeAttributes(TypeAttributes attributes, TypeMember type)
		{
			switch (type.NodeType)
			{
				case NodeType.ClassDefinition:
					{
						attributes |= (TypeAttributes.AnsiClass | TypeAttributes.AutoLayout);
						attributes |= TypeAttributes.Class;

						if (!((ClassDefinition)type).HasDeclaredStaticConstructor)
						{
							attributes |= TypeAttributes.BeforeFieldInit;
						}
						if (type.IsAbstract)
						{
							attributes |= TypeAttributes.Abstract;
						}
						if (type.IsFinal)
						{
							attributes |= TypeAttributes.Sealed;
						}

						if (type.IsStatic) //static type is Sealed+Abstract in SRE
							attributes |= TypeAttributes.Sealed | TypeAttributes.Abstract;
						else if (!type.IsTransient)
							attributes |= TypeAttributes.Serializable;

						if (((IType)type.Entity).IsValueType)
						{
							attributes |= TypeAttributes.SequentialLayout;
						}
						break;
					}

				case NodeType.EnumDefinition:
					{
						attributes |= TypeAttributes.Sealed;
						attributes |= TypeAttributes.Serializable;
						break;
					}

				case NodeType.InterfaceDefinition:
					{
						attributes |= (TypeAttributes.Interface | TypeAttributes.Abstract);
						break;
					}

				case NodeType.Module:
					{
						attributes |= TypeAttributes.Sealed;
						break;
					}
			}
			return attributes;
		}

        public void AddInterfaceImplementation(IType intf)
        {
			_interfaceImplementations.Add(intf);
		}

		internal void SetLayout(TypeAttributes layout)
        {
			if ((layout & TypeAttributes.LayoutMask) != layout)
            {
				throw new EcmaBuildException($"Non-layout value '{layout}' passed to SetLayout");
            }
			_attrs = (_attrs & ~TypeAttributes.LayoutMask) | layout;
        }

		private void EmitBaseTypes()
		{
			foreach (var baseType in _type.BaseTypes)
			{
				var type = (IType)baseType.Entity;
				if ((type.ConstructedInfo != null && type.ConstructedInfo.GenericDefinition.IsClass) || (type.IsClass))
				{
					_parent = type;
				}
				else
				{
					_interfaceImplementations.Add(type);
				}
			}
			if (_type is EnumDefinition)
            {
				_parent = _typeSystem.EnumTypeEntity;
            } 
			else if (_type is StructDefinition)
            {
				_parent = _typeSystem.ValueTypeEntity;
            }
		}

        internal void AddMethodImplementation(EntityHandle ifaceInfo, EntityHandle implInfo)
        {
			_explicitImpls.Add(KeyValuePair.Create(ifaceInfo, implInfo));
        }

		public void Build()
        {
			EmitBaseTypes();
			var asm = _typeSystem.AssemblyBuilder;
			var firstField = _fields.FirstOrDefault();
			var firstMethod = _methods.FirstOrDefault();
			var handle = asm.AddTypeDefinition(
				_attrs,
				_type.EnclosingNamespace == null ? default : asm.GetOrAddString(_type.EnclosingNamespace.Name),
				asm.GetOrAddString(_name),
				_parent == null ? (_type.NodeType == NodeType.InterfaceDefinition ? default : _typeSystem.ObjectType) : _typeSystem.LookupType(_parent),
				firstField != null ? (FieldDefinitionHandle)firstField.Handle : MetadataTokens.FieldDefinitionHandle(asm.GetRowCount(TableIndex.Field) + 1),
				firstMethod != null ? (MethodDefinitionHandle)firstMethod.Handle : MetadataTokens.MethodDefinitionHandle(asm.GetRowCount(TableIndex.MethodDef) + 1)
			);
			if (handle != _handle)
            {
				throw new EcmaBuildException($"Type handle {handle} does not match reserved handle {_handle}.");
            }
			foreach (var field in _fields)
            {
				field.Build();
            }
			foreach (var method in _methods)
            {
				method.Build();
            }
			foreach (var prop in _properties)
            {
				prop.Build();
            }
			foreach (var ev in _events)
            {
				ev.Build();
            }
			foreach (var iHandle in _interfaceImplementations.Select(_typeSystem.LookupType).OrderBy(CodedIndex.TypeDefOrRefOrSpec))
            {
				asm.AddInterfaceImplementation(handle, iHandle);
            }
			foreach (var exp in _explicitImpls)
            {
				asm.AddMethodImplementation(handle, exp.Value, exp.Key);
            }
			foreach (var nested in _nestedTypes)
            {
				_typeSystem.RegisterNestedType(Handle, nested);
            }
        }

        internal GenericTypeParameterBuilder[] DefineGenericParameters(IEnumerable<IGenericParameter> parameters)
        {
			if (_genParams != null)
            {
				throw new EcmaBuildException("Generic parameters have already been defined for this type.");
            }
			var result = parameters.Select(p => new GenericTypeParameterBuilder(p, this, _typeSystem)).ToArray();
			_genParams = result;
			return result;
		}
    }
}
