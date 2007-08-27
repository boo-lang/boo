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
		int _position = -1;
		TypeDefinition _declaringType;
		Method _declaringMethod;
		GenericParameterDeclaration _declaration;

		IType[] _emptyTypeArray = new IType[0];
		
		public InternalGenericParameter(TypeSystemServices tss, GenericParameterDeclaration declaration)
		{
			_tss = tss;
			_declaration = declaration;

			// Determine and remember declaring type and declaring method (if applicable)
			_declaringMethod = declaration.ParentNode as Method;
			_declaringType = (
				_declaringMethod == null ? 
				declaration.ParentNode as TypeDefinition : _declaringMethod.DeclaringType);
		}

		public int GenericParameterPosition
		{
			get 
			{
				if (_position == -1)
				{
					IGenericParameter[] parameters = 
						DeclaringMethod == null ? DeclaringMethod.GenericInfo.GenericParameters : DeclaringType.GenericInfo.GenericParameters;
					
					_position = Array.IndexOf(parameters, this);
				}

				return _position;
			}
		}
		
		public IType DeclaringType
		{
		 	get 
		 	{ 
		 		return (IType)_declaringType.Entity; 
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
				return ((Node)_declaringMethod ?? (Node)_declaringType).Entity;
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
			return _emptyTypeArray;
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