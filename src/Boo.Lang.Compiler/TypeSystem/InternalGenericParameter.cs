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
// CAUSED AND TODON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Reflection;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem
{
    /// <summary>
    /// A generic type parameter of an internal generic type or method.
    /// </summary>
	public class InternalGenericParameter : IType, IInternalEntity, IGenericParameter
	{
		TypeSystemServices _tss;
		int _position;
		string _name;
		IType _declaringType = null;
		IMethod _declaringMethod = null;
		GenericParameterDeclaration _declaration;
		
		private InternalGenericParameter(TypeSystemServices tss, GenericParameterDeclaration declaration, int position)
		{
			_tss = tss;
			_declaration = declaration;
			_name = declaration.Name;
			_position = position;
		}

		public InternalGenericParameter(TypeSystemServices tss, GenericParameterDeclaration declaration, AbstractInternalType declaringType, int position)
			: this(tss, declaration, position)
		{			
			_declaringType = declaringType;
		}

		public InternalGenericParameter(TypeSystemServices tss, GenericParameterDeclaration declaration, InternalMethod declaringMethod, int position) 
			: this(tss, declaration, position)
		{
			_declaringMethod = declaringMethod;
		}
				
		public int GenericParameterPosition
		{
			get { return _position; }
		}
		
		public IType DeclaringType
		{
		 	get 
		 	{ 
		 		if (_declaringType == null)
		 		{
		 			_declaringType = _declaringMethod.DeclaringType;
		 		}
		 		return _declaringType; 
		 	}
		}
		
		public IMethod DeclaringMethod
		{
			get { return _declaringMethod; }
		}
		
		public IEntity DeclaringEntity
		{
			get 
			{ 
				return _declaringMethod == null ? 
					(IEntity)_declaringType : (IEntity)_declaringMethod;
			}
		}
		
		public bool IsClass
		{
			get { return false; }
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
		
		public bool IsValueType
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
			// TODO: Return base constraint or system.object
			get { return _tss.ObjectType; }
		}
		
		public IEntity GetDefaultMember()
		{
			return null;
		}
		
		public IConstructor[] GetConstructors()
		{
			return null;
		}
		
		public IType[] GetInterfaces()
		{
			// TODO: return interface constraints and inherited interfaces
			return null;
		}
		
		public bool IsSubclassOf(IType other)
		{
			return (other == BaseType || BaseType.IsSubclassOf(other));
		}
		
		public bool IsAssignableFrom(IType other)
		{
			return (other == this);
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
			get { return _name; }
		}

		public string FullName 
		{
			get 
			{
				return string.Format("{0}.{1}", DeclaringEntity.FullName, _name);
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
			// Resolve using base type constraint
			if (BaseType != null)
			{
				if (BaseType.Resolve(targetList, name, flags))
				{
					return true;
				}
			}
			
			// Resolve using interface constraints
			foreach (IType type in GetInterfaces())
			{
				if (type.Resolve(targetList, name, flags))
				{
					return true;
				}
			}
			
			return false;
		}
		
		public Node Node
		{
			get { return _declaration; }
		}
		
		override public string ToString()
		{
			return FullName;
		}

	}
}