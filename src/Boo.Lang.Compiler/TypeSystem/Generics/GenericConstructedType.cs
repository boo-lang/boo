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

using System;
using System.Collections.Generic;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem.Generics
{
	/// <summary>
	/// A type constructed by supplying type parameters to a generic type, involving internal types.
	/// </summary>
	/// <remarks>
	/// Constructed types constructed from an external generic type with external type arguments 
	/// are themselves external, and are represented as ExternalType instances. All other cases
	/// are represented by this type.
	/// </remarks>
	public class GenericConstructedType : IType, IConstructedTypeInfo
	{
		protected IType _definition;
		IType[] _arguments;
		GenericMapping _genericMapping;
		bool _fullyConstructed;
        
		string _fullName = null;

		public GenericConstructedType(IType definition, IType[] arguments)
		{
			_definition = definition;
			_arguments = arguments;
			_genericMapping = new InternalGenericMapping(this, arguments);
			_fullyConstructed = IsFullyConstructed();
		}

		protected bool IsFullyConstructed()
		{
			return GenericsServices.GetTypeGenerity(this) == 0;
		}

		protected string BuildFullName()
		{
			string baseName = _definition.FullName;
			int typeParametersPosition = baseName.LastIndexOf("[");
			if (typeParametersPosition >= 0) baseName = baseName.Remove(typeParametersPosition);

			string[] argumentNames = Array.ConvertAll<IType, string>(
				ConstructedInfo.GenericArguments,
				delegate(IType t) { return t.FullName; });

			return string.Format("{0}[of {1}]", baseName, string.Join(", ", argumentNames));
		}

		protected GenericMapping GenericMapping
		{
			get { return _genericMapping; }
		}

		public IEntity DeclaringEntity
		{
			get { return _definition.DeclaringEntity;  }
		}

		public bool IsClass
		{
			get { return _definition.IsClass; }
		}

		public bool IsAbstract
		{
			get { return _definition.IsAbstract; }
		}

		public bool IsInterface
		{
			get { return _definition.IsInterface; }
		}

		public bool IsEnum
		{
			get { return _definition.IsEnum; }
		}

		public bool IsByRef
		{
			get { return _definition.IsByRef; }
		}

		public bool IsValueType
		{
			get { return _definition.IsValueType; }
		}

		public bool IsFinal
		{
			get { return _definition.IsFinal; }
		}

		public bool IsArray
		{
			get { return _definition.IsArray; }
		}

		public bool IsPointer
		{
			get { return _definition.IsPointer; }
		}

		public int GetTypeDepth()
		{
			return _definition.GetTypeDepth();
		}

		public IType GetElementType()
		{
			return GenericMapping.MapType(_definition.GetElementType());
		}

		public IType BaseType
		{
			get { return GenericMapping.MapType(_definition.BaseType); }
		}

		public IEntity GetDefaultMember()
		{
			IEntity definitionDefaultMember = _definition.GetDefaultMember();
			if (definitionDefaultMember != null) return GenericMapping.Map(definitionDefaultMember);
			return null;
		}

		public IConstructor[] GetConstructors()
		{
			return Array.ConvertAll<IConstructor, IConstructor>(
				_definition.GetConstructors(),
				delegate(IConstructor c) { return GenericMapping.Map(c); });
		}

		public IType[] GetInterfaces()
		{
			return Array.ConvertAll<IType, IType>(
				_definition.GetInterfaces(), 
				GenericMapping.MapType);
		}

		public bool IsSubclassOf(IType other)
		{
			if (null == other)
				return false;

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

			if (null != other.ConstructedInfo
			    && ConstructedInfo.GenericDefinition == other.ConstructedInfo.GenericDefinition)
			{
				for (int i = 0; i < ConstructedInfo.GenericArguments.Length; ++i)
				{
					if (!ConstructedInfo.GenericArguments[i].IsSubclassOf(other.ConstructedInfo.GenericArguments[i]))
						return false;
				}
				return true;
			}

			return false;
		}

		public virtual bool IsAssignableFrom(IType other)
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

		public IGenericTypeInfo GenericInfo
		{
			get { return _definition.GenericInfo; }
		}

		public IConstructedTypeInfo ConstructedInfo
		{
			get { return this; }
		}

		public IType Type
		{
			get { return this; }
		}

		public INamespace ParentNamespace
		{
			get 
			{              
				return GenericMapping.Map(_definition.ParentNamespace as IEntity) as INamespace; 
			}
		}

		public bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
		{
			Set<IEntity> definitionMatches = new Set<IEntity>();
			if (!_definition.Resolve(definitionMatches, name, typesToConsider))
				return false;
			foreach (IEntity match in definitionMatches)
				resultingSet.Add(GenericMapping.Map(match));
			return true;
		}

		public IEnumerable<IEntity> GetMembers()
		{
			return Collections.Select<IEntity, IEntity>(_definition.GetMembers(), GenericMapping.Map);
		}

		public string Name
		{
			get { return _definition.Name; }
		}

		public string FullName
		{
			get { return _fullName ?? (_fullName = BuildFullName()); }
		}

		public EntityType EntityType
		{
			get { return EntityType.Type; }
		}

		IType[] IConstructedTypeInfo.GenericArguments
		{
			get { return _arguments; }
		}

		IType IConstructedTypeInfo.GenericDefinition
		{
			get { return _definition; }
		}

		bool IConstructedTypeInfo.FullyConstructed
		{
			get { return _fullyConstructed; }
		}

		IType IConstructedTypeInfo.Map(IType type)
		{
			return GenericMapping.MapType(type);
		}

		IMember IConstructedTypeInfo.Map(IMember member)
		{
			return (IMember)GenericMapping.Map(member);
		}

		IMember IConstructedTypeInfo.UnMap(IMember mapped)
		{
			return GenericMapping.UnMap(mapped);
		}

		public bool IsDefined(IType attributeType)
		{
			return _definition.IsDefined(GenericMapping.MapType(attributeType));
		}

		public override string ToString()
		{
			return FullName;
		}

		private Memo<int, IArrayType> _arrayTypes;

		public IArrayType MakeArrayType(int rank)
		{
			if (null == _arrayTypes)
				_arrayTypes = new Memo<int, IArrayType>();
			return _arrayTypes.Produce(rank, delegate(int newRank)
			{
				return new ArrayType(this, newRank);
			});
		}

		public IType MakePointerType()
		{
			return null;
		}
	}

	public class GenericConstructedCallableType : GenericConstructedType, ICallableType
	{
		CallableSignature _signature;

		public GenericConstructedCallableType(ICallableType definition, IType[] arguments) 
			: base(definition, arguments) 
		{
		}

		public CallableSignature GetSignature()
		{
			if (_signature == null)
			{
				CallableSignature definitionSignature = ((ICallableType)_definition).GetSignature();
                
				IParameter[] parameters = GenericMapping.MapParameters(definitionSignature.Parameters);
				IType returnType = GenericMapping.MapType(definitionSignature.ReturnType);
                
				_signature = new CallableSignature(parameters, returnType);
			}

			return _signature;
		}

		override public bool IsAssignableFrom(IType other)
		{
			return My<TypeSystemServices>.Instance.IsCallableTypeAssignableFrom(this, other);
		}
	}
}
