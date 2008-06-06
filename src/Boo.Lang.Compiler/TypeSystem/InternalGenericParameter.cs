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
using Boo.Lang.Compiler.Ast;
using System.Collections.Generic;

namespace Boo.Lang.Compiler.TypeSystem
{
    /// <summary>
    /// A generic type parameter of an internal generic type or method.
    /// </summary>
	public class InternalGenericParameter : IType, IInternalEntity, IGenericParameter
	{
		int _position = -1;
		GenericParameterDeclaration _declaration;
		TypeSystemServices _tss;

		IType[] _baseTypes = null;
		
		public InternalGenericParameter(TypeSystemServices tss, GenericParameterDeclaration declaration)
		{
			_tss = tss;
			_declaration = declaration;
		}

		public InternalGenericParameter(TypeSystemServices tss, GenericParameterDeclaration declaration, int position)
				: this(tss, declaration)
		{
			_position = position;
		}

		public int GenericParameterPosition
		{
			get 
			{
				if (_position == -1)
				{
					IGenericParameter[] parameters = 
						DeclaringMethod != null ? DeclaringMethod.GenericInfo.GenericParameters : DeclaringType.GenericInfo.GenericParameters;
					
					_position = Array.IndexOf(parameters, this);
				}

				return _position;
			}
		}

		public bool MustHaveDefaultConstructor
		{
			get
			{
				return HasConstraint(GenericParameterConstraints.Constructable);
			}
		}

		public Variance Variance
		{
			get 
			{
				if (HasConstraint(GenericParameterConstraints.Covariant))
				{
					return Variance.Covariant;
				}
				else if (HasConstraint(GenericParameterConstraints.Contravariant))
				{
					return Variance.Contravariant;
				}
				return Variance.Invariant;
			}
		}

		public IType[] GetTypeConstraints()
		{
			if (_baseTypes == null)
			{
				List<IType> baseTypes = new List<IType>();

				foreach (TypeReference baseTypeReference in _declaration.BaseTypes)
				{
					IType baseType = (IType)baseTypeReference.Entity;
					if (baseType != null)
					{
						baseTypes.Add(baseType);
					}
				}

				_baseTypes = baseTypes.ToArray();
			}
			
			return _baseTypes;			
		}

		public bool IsValueType
		{
			get { return HasConstraint(GenericParameterConstraints.ValueType); }
		}

		public bool IsClass
		{
			get { return HasConstraint(GenericParameterConstraints.ReferenceType); }
		}

		public IType DeclaringType
		{
		 	get 
		 	{
		 		return DeclaringEntity as IType ??
					((IMethod)DeclaringEntity).DeclaringType;
		 	}
		}
		
		public IMethod DeclaringMethod
		{
			get { return DeclaringEntity as IMethod; } 
		}
		
		public IEntity DeclaringEntity
		{
			get 
			{
				return TypeSystemServices.GetEntity(_declaration.ParentNode);
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
			if (MustHaveDefaultConstructor)
			{
				// TODO: return a something implementing IConstructor...?
			}
			return null;
		}
		
		public IType[] GetInterfaces()
		{
			List<IType> interfaces = new List<IType>();

			foreach (IType type in GetTypeConstraints())
			{
				if (type.IsInterface)
				{
					interfaces.Add(type);
				}
			}
		
			return interfaces.ToArray();
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

		public string Name
		{
			get { return _declaration.Name; }
		}

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
			get { return (INamespace)DeclaringEntity; }
		}
		
		IEntity[] INamespace.GetMembers()
		{
			return NullNamespace.EmptyEntityArray;
		}
		
		bool INamespace.Resolve(List targetList, string name, EntityType flags)
		{
			bool resolved = false;
			
			// Resolve using base type constraints
			foreach (IType type in GetTypeConstraints())
			{
				resolved |= type.Resolve(targetList, name, flags);
			}
			
			// Resolve using System.Object
			resolved |= _tss.ObjectType.Resolve(targetList, name, flags);

			return resolved;
		}
		
		public Node Node
		{
			get { return _declaration; }
		}
		
		override public string ToString()
		{
			return FullName;
		}
				
		private bool HasConstraint(GenericParameterConstraints flag)
		{
			return (_declaration.Constraints & flag) == flag;
		}

		private IType FindBaseType()
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

		override public bool Equals(object rhs)
		{
			IGenericParameter p = rhs as IGenericParameter;
			if (null == p) return false;

			//TODO: >=0.9 : check base type constraints
			return Name == p.Name //FIXME: should be GenericParameterPosition but crashes on internal g. params(?!!)
				&& Variance == p.Variance
				&& MustHaveDefaultConstructor  == p.MustHaveDefaultConstructor
				&& IsClass == p.IsClass
				&& IsValueType == p.IsValueType;
		}
		
		override public int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
