#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
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

#if NET_2_0

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Text;
	using System.Reflection;
	using System.Collections.Generic;

	/// <summary>
	/// A generic type constructed from an external definition but involving internal parameters.
	/// </summary>
	public class MixedGenericType : ExternalType, IGenericTypeInfo
	{
		#region Data Members

		ExternalType _definition;
		IType[] _arguments = null;
		bool _constructed;
		string _name = null;
		string _fullName = null;
		Dictionary<IEntity, IEntity> _mappedMembers = new Dictionary<IEntity, IEntity>();
		
		#endregion
		
		#region Constructor

		public MixedGenericType(TypeSystemServices tss, ExternalType definition, IType[] arguments) : base(tss, definition.ActualType)
		{
			_definition = definition;
			_arguments = arguments;
			_constructed = IsConstructed();
		}
		
		#endregion

		#region IGenericTypeInfo members
		
		public IType[] GenericArguments
		{
			get { return _arguments; }
		}
		
		public IType GenericDefinition
		{
			get { return _definition; }
		}

		public bool FullyConstructed
		{
			get { return _constructed; }
		}
		
		#endregion
		
		#region Properties

		public override IGenericTypeInfo GenericTypeInfo
		{
			get { return this; }
		}

		public override IType BaseType
		{
			get { return MapType(_definition.BaseType); }
		}
		
		public override string Name
		{
			get
			{
				if (_name == null)
				{
					_name = BuildName(false);
				}
				return _name;
			}
		}
		
		public override string FullName
		{
			get
			{
				if (_fullName == null)
				{
					_fullName = BuildName(true);
				}
				return _fullName;
			}
		}
		
		#endregion

		#region Private Methods

		private bool IsConstructed()
		{
			foreach (IType arg in _arguments)
			{
				if (arg is IGenericParameter) return false;
			}
			
			return true;
		}
		
		private string BuildName(bool full)
		{
			Converter<IType, string> argumentName = delegate(IType type)
			{
				return full ? "[" + type.FullName + "]" : type.Name;
			};
			
			string[] typeNames = Array.ConvertAll(_arguments, argumentName);
			
			return string.Format(
				"{0}[{1}]",
				full ? _definition.FullName : _definition.Name,
				string.Join(", ", typeNames));
		}
		
		#endregion

		#region Public Methods

		public override string ToString()
		{
			return FullName;
		}
		
		public override IType GetElementType()
		{
			return MapType(_definition.GetElementType());
		}
		
		public override IEntity GetDefaultMember()
		{
			return MapMember(_definition.GetDefaultMember());
		}
		
		public override IConstructor[] GetConstructors()
		{
			return Array.ConvertAll<IConstructor, IConstructor>(
				_definition.GetConstructors(),
				delegate(IConstructor c) { return (IConstructor)MapMember(c); });
		}
		
		public override IType[] GetInterfaces()
		{
			return Array.ConvertAll<IType, IType>(
				_definition.GetInterfaces(),
				MapType);
		}
		
		public override IEntity[] GetMembers()
		{
			return Array.ConvertAll<IEntity, IEntity>(
				_definition.GetMembers(),
				MapMember);
		}
		
		public override bool IsSubclassOf(IType other)
		{
			if (BaseType != null && (BaseType == other || BaseType.IsSubclassOf(other)))
			{
				return true;
			}
			
			if (other.IsInterface && Array.Exists(
				GetInterfaces(),
				delegate(IType i) { return other.IsAssignableFrom(i); }))
			{
				return true;
			}
			
			return false;
		}
		
		public override bool IsAssignableFrom(IType other)
		{
			if (other == null)
			{
				return false;
			}
			
			if (other == this || other.IsSubclassOf(this) || (other == Null.Default && !IsValueType))
			{
				return true;
			}
			
			return false;
		}
		
		#endregion
		
		#region Mapping methods
		
		/// <summary>
		/// Maps a type involving generic parameters to the corresponding type after substituting concrete
		/// arguments for generic parameters.
		/// </summary>
		/// <remarks>
		/// If the source type is a generic parameter of this type's definition, it is mapped to the
		/// corresponding argument.
		/// If the source type is an open generic type using parameters from the type's definition, it
		/// is mapped to a closed constructed type based on this type's arguments.
		/// If the source type is an array of a generic parameter of this type's definition, it is mapped
		/// to the array type of the corresponding argument, of the same rank.
		/// </remarks>
		protected IType MapType(IType sourceType)
		{
			if (sourceType == null)
			{
				return null;
			}
			
			// If sourceType is a reference type, map its element type 
			if (sourceType.IsByRef)
			{
				return MapType(sourceType.GetElementType());
			}

			// Map generic parameter to corresponding argument
			IGenericParameter gp = sourceType as IGenericParameter;
			if (null != gp && gp.DeclaringType == _definition)
			{
				return GenericArguments[gp.GenericParameterPosition];
			}

			// Map own definition to this mixed type
			if (sourceType == GenericDefinition)
			{
				return this;
			}
			
			// Map open constructed type using generic parameters to closed constructed type
			// using corresponding arguments
			if (null != sourceType.GenericTypeInfo)
			{
				IType[] mappedArguments = Array.ConvertAll<IType, IType>(
					sourceType.GenericTypeInfo.GenericArguments,
					MapType);
				
				IType mapped = sourceType.GenericTypeInfo.
					GenericDefinition.GenericTypeDefinitionInfo.
					MakeGenericType(mappedArguments);
				
				return mapped;
			}
			
			// Map array of generic parameter to array of corresponding argument
			IArrayType array = (sourceType as IArrayType);
			if (array != null)
			{
				return _typeSystemServices.GetArrayType(MapType(array.GetElementType()), array.GetArrayRank());
			}
			
			// If source type doesn't require mapping, return it as is
			return sourceType;
		}
		
		/// <summary>
		/// Maps a member from the type's definition to its constructed version.
		/// </summary>
		public IEntity MapMember(IEntity source)
		{
			if (source == null) return null;
			
			if (_mappedMembers.ContainsKey(source))
			{
				return _mappedMembers[source];
			}
			
			IEntity mapped = null;
			
			switch (source.EntityType)
			{
				case EntityType.Method:
					mapped = new MappedMethod(_typeSystemServices, ((ExternalMethod)source).MethodInfo, this);
					break;
					
				case EntityType.Constructor:
					mapped = new MappedConstructor(_typeSystemServices, ((ExternalConstructor)source).ConstructorInfo, this);
					break;
					
				case EntityType.Field:
					mapped = new MappedField(_typeSystemServices, ((ExternalField)source).FieldInfo, this);
					break;
					
				case EntityType.Property:
					mapped = new MappedProperty(_typeSystemServices, ((ExternalProperty)source).PropertyInfo, this);
					break;
					
				case EntityType.Type:
					mapped = MapType((IType)source);
					break;
					
				case EntityType.Event:
					mapped = new MappedEvent(_typeSystemServices, ((ExternalEvent)source).EventInfo, this);
					break;
					
				default:
					throw new ArgumentException(
						string.Format("Invalid entity type for mapping: {0}.", source.EntityType));
			}
			
			_mappedMembers[source] = mapped;
			return mapped;
		}
		
		#endregion

		#region class MappedMethod
		
		/// <summary>
		/// A method in a mixed generic type.
		/// </summary>
		public class MappedMethod : ExternalMethod
		{
			private MixedGenericType _parentType;
			
			public MappedMethod(TypeSystemServices tss, MethodBase method, MixedGenericType parentType) : base(tss, method)
			{
				_parentType = parentType;
			}
			
			public override IType DeclaringType
			{
				// get { return _parentType; }
				get { return _parentType.MapType(base.DeclaringType); }
			}
			
			public IType MixedType
			{
				get { return _parentType; }
			}
			public override IType ReturnType
			{
				get
				{
					return _parentType.MapType(base.ReturnType);
				}
			}
			
			public override IParameter[] GetParameters()
			{
				return Array.ConvertAll<IParameter, MappedParameter>(
					base.GetParameters(),
					delegate(IParameter p)
					{
						return new MappedParameter(_typeSystemServices, (ExternalParameter)p, _parentType);
					});
			}			
		}
		
		#endregion
		
		#region class MappedConstructor
		
		/// <summary>
		/// A constructor in a mixed generic type.
		/// </summary>
		public class MappedConstructor : MappedMethod, IConstructor
		{
			public MappedConstructor(TypeSystemServices tss, ConstructorInfo ci, MixedGenericType parentType) : base(tss, ci, parentType)
			{
			}

			public override EntityType EntityType
			{
				get { return EntityType.Constructor; }
			}
			
			public override IType ReturnType
			{
				get { return _typeSystemServices.VoidType; }
			}
			
			public ConstructorInfo ConstructorInfo
			{
				get
				{
					return (ConstructorInfo)MethodInfo;
				}
			}
		}
		
		#endregion

		#region class MappedParameter
		
		/// <summary>
		/// A parameter in a method or constructor of a mixed generic type.
		/// </summary>
		public class MappedParameter : IParameter
		{
			private MixedGenericType _parentType;
			private ExternalParameter _baseParameter;
			
			public MappedParameter(TypeSystemServices tss, ExternalParameter parameter, MixedGenericType parentType)
			{
				_parentType = parentType;
				_baseParameter = parameter;
			}
			
			public bool IsByRef
			{
				get { return _baseParameter.IsByRef; }
			}
			
			public IType Type
			{
				get { return _parentType.MapType(_baseParameter.Type); }
			}
			
			public string Name
			{
				get { return _baseParameter.Name; }
			}
			
			public string FullName
			{
				get { return _baseParameter.FullName; }
			}
			
			public EntityType EntityType
			{
				get { return EntityType.Parameter; }
			}
		}
		
		#endregion
		
		#region class MappedProperty
		
		public class MappedProperty : ExternalProperty
		{
			private MixedGenericType _parentType;
			
			public MappedProperty(TypeSystemServices tss, PropertyInfo property, MixedGenericType parentType) : base(tss, property)
			{
				_parentType = parentType;
			}
			
			public override IType Type
			{
				get { return _parentType.MapType(base.Type); }
			}

			public override IType DeclaringType
			{
				get { return _parentType.MapType(base.DeclaringType); }
			}
			
			public override IParameter[] GetParameters()
			{
				IParameter[] baseParams = base.GetParameters();
				return Array.ConvertAll<IParameter, MappedParameter>(
					baseParams,
					delegate(IParameter p)
					{
						return new MappedParameter(_typeSystemServices, (ExternalParameter)p, _parentType);
					});
			}
			
			public override IMethod GetGetMethod()
			{
				return (IMethod)_parentType.MapMember(base.GetGetMethod());
			}
			
			public override IMethod GetSetMethod()
			{
				return (IMethod)_parentType.MapMember(base.GetSetMethod());
			}
			
			public override string ToString()
			{
				IParameter[] parameters = GetParameters();
				if (parameters.Length > 0)
				{
					string[] parameterNames = Array.ConvertAll<IParameter, string>(
						parameters,
						delegate(IParameter p) { return p.Type.Name; });
					
					return string.Format(
						"{0} {1} [{2}]",
						Type.Name,
						Name,
						string.Join(", ", parameterNames));
				}
				
				return string.Format("{0} {1}", Type.Name, Name);
			}
		}
		#endregion

		#region class MappedEvent
		
		public class MappedEvent : ExternalEvent
		{
			private MixedGenericType _parentType;

			public MappedEvent(TypeSystemServices tss, EventInfo evt, MixedGenericType parentType) : base(tss, evt)
			{
				_parentType = parentType;
			}
			
			public override IType DeclaringType
			{
				get { return _parentType.MapType(base.DeclaringType); }
			}
			
			public override IMethod GetAddMethod()
			{
				return (IMethod)_parentType.MapMember(base.GetAddMethod());
			}
			
			public override IMethod GetRemoveMethod()
			{
				return (IMethod)_parentType.MapMember(base.GetRemoveMethod());
			}
			
			public override IMethod GetRaiseMethod()
			{
				return (IMethod)_parentType.MapMember(base.GetRaiseMethod());
			}
			
			public override IType Type
			{
				get { return _parentType.MapType(base.Type); }
			}
		}
		
		#endregion
		
		#region class MappedField
		
		public class MappedField : ExternalField
		{
			MixedGenericType _parentType;
			
			public MappedField(TypeSystemServices tss, FieldInfo field, MixedGenericType parentType) : base(tss, field)
			{
				_parentType = parentType;
			}
			
			public override IType DeclaringType
			{
				get { return _parentType.MapType(base.DeclaringType); }
			}
			
			public override IType Type
			{
				get { return _parentType.MapType(base.Type); }
			}
		}
		
		#endregion
	}
	
	public class MixedGenericCallableType: MixedGenericType, ICallableType
	{
		ExternalCallableType _definition;
		CallableSignature _signature;
		
		public MixedGenericCallableType(TypeSystemServices tss, ExternalCallableType definition, IType[] arguments):
			base(tss, definition, arguments)
		{
			_definition = definition;
		}

		public CallableSignature GetSignature()
		{
			if (_signature == null)
			{
				CallableSignature definitionSignature = _definition.GetSignature();

				IParameter[] parameters = Array.ConvertAll<IParameter, IParameter>(
					definitionSignature.Parameters,
					delegate(IParameter p)
					{
						return new MappedParameter(_typeSystemServices, (ExternalParameter)p, this);
					});
				
				_signature = new CallableSignature(parameters, MapType(definitionSignature.ReturnType));
			}
			
			return _signature;
		}

		override public bool IsAssignableFrom(IType other)
		{
			return _typeSystemServices.IsCallableTypeAssignableFrom(this, other);
		}
	}
}

#endif
