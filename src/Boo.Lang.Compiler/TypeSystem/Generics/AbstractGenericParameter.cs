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
	public abstract class AbstractGenericParameter : IGenericParameter
	{
		TypeSystemServices _tss;

		protected AbstractGenericParameter(TypeSystemServices tss)
		{
			_tss = tss;
		}

		abstract public int GenericParameterPosition { get; }

		abstract public bool MustHaveDefaultConstructor { get; }

		abstract public Variance Variance { get; }

		abstract public bool IsClass { get; }

		abstract public bool IsValueType { get; }

		abstract public IType[] GetTypeConstraints();

		abstract public IEntity DeclaringEntity { get; }

		protected IType DeclaringType
		{
			get
			{
				if (DeclaringEntity is IType)
				{
					return (IType)DeclaringEntity;
				}

				if (DeclaringEntity is IMethod)
				{
					return ((IMethod)DeclaringEntity).DeclaringType;
				}

				return null;
			}
		}

		protected IMethod DeclaringMethod 
		{
			get
			{
				return DeclaringEntity as IMethod;
			}
		}

		bool IType.IsAbstract
		{
			get { return false; }
		}
		
		bool IType.IsInterface
		{
			get { return false; }
		}
		
		bool IType.IsEnum
		{
			get { return false; }
		}
		
		public bool IsByRef
		{
			get { return false; }
		}
		
		bool IType.IsFinal
		{
			get { return true; }
		}
		
		bool IType.IsArray
		{
			get { return false; }
		}
		bool IType.IsPointer
		{
			get { return false; }
		}

		public int GetTypeDepth()
		{
			return DeclaringType.GetTypeDepth() + 1;
		}
		
		IType IType.GetElementType()
		{
			return null;
		}

		public IType BaseType
		{
			get { return FindBaseType(); }
		}
		
		public IEntity GetDefaultMember()
		{
			return null;
		}
		
		public IConstructor[] GetConstructors()
		{
			return new IConstructor[0];
		}
		
		public IType[] GetInterfaces()
		{
			return Array.FindAll(GetTypeConstraints(), delegate(IType type) { return type.IsInterface; });
		}

		public bool IsSubclassOf(IType other)
		{
			return (other == BaseType || BaseType.IsSubclassOf(other));
		}
		
		public bool IsAssignableFrom(IType other)
		{
			if (other == this)
			{
				return true;
			}
		
			if (other == Null.Default)
			{
				return IsClass;
			}

			IGenericParameter otherParameter = other as IGenericParameter;
			if (otherParameter != null && Array.Exists(otherParameter.GetTypeConstraints(), IsAssignableFrom))
			{
				return true;
			}

			return false;
		}
		
		IGenericTypeInfo IType.GenericInfo 
		{ 
			get { return null; } 
		}
		
		IConstructedTypeInfo IType.ConstructedInfo 
		{ 
			get { return null; } 
		}

		abstract public string Name { get; }

		public string FullName 
		{
			get 
			{
				return string.Format("{0}.{1}", DeclaringEntity.FullName, Name);
			}
		}
		
		public EntityType EntityType
		{
			get { return EntityType.Type; }
		}
		
		public IType Type
		{
			get { return this; }
		}
		
		INamespace INamespace.ParentNamespace
		{
			get { return DeclaringType; }
		}
		
		IEnumerable<IEntity> INamespace.GetMembers()
		{
			return NullNamespace.EmptyEntityArray;
		}
		
		bool INamespace.Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
		{
			bool resolved = false;
			
			foreach (IType type in GetTypeConstraints())
			{
				resolved |= type.Resolve(resultingSet, name, typesToConsider);
			}
			
			resolved |= _tss.ObjectType.Resolve(resultingSet, name, typesToConsider);

			return resolved;
		}
		
		override public string ToString()
		{
			return Name;
		}

		bool IEntityWithAttributes.IsDefined(IType attributeType)
		{
			throw new NotImplementedException();
		}

		protected IType FindBaseType()
		{
			foreach (IType type in GetTypeConstraints())
			{
				if (!type.IsInterface)
				{
					return type;
				}
			}
			return _tss.ObjectType;
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
}
