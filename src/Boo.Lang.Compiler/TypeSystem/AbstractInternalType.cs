#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Collections;
	using Boo.Lang.Compiler.Ast;
	
	public abstract class AbstractInternalType : IInternalEntity, IType, INamespace
	{		
		protected TypeSystemServices _typeSystemServices;
		
		protected TypeDefinition _typeDefinition;
		
		protected IEntity[] _members;
		
		protected IType[] _interfaces;
		
		protected INamespace _parentNamespace;
		
		protected Boo.Lang.List _buffer = new Boo.Lang.List();
		
		protected AbstractInternalType(TypeSystemServices typeSystemServices, TypeDefinition typeDefinition)
		{
			_typeSystemServices = typeSystemServices;
			_typeDefinition = typeDefinition;			
		}
		
		public string FullName
		{
			get
			{
				return _typeDefinition.FullName;
			}
		}
		
		public string Name
		{
			get
			{
				return _typeDefinition.Name;
			}
		}	
		
		public Node Node
		{
			get
			{
				return _typeDefinition;
			}
		}
		
		public virtual INamespace ParentNamespace
		{
			get
			{
				if (null == _parentNamespace)
				{
					_parentNamespace = (INamespace)TypeSystemServices.GetEntity(_typeDefinition.ParentNode);
				}
				return _parentNamespace;
			}
		}
		
		public virtual bool Resolve(Boo.Lang.List targetList, string name, EntityType flags)
		{			
			bool found = false;
			
			foreach (IEntity tag in GetMembers())
			{
				if (tag.Name == name && NameResolutionService.IsFlagSet(flags, tag.EntityType))
				{
					targetList.AddUnique(tag);
					found = true;
				}
			}
			
			if (!found)
			{			
				foreach (TypeReference baseType in _typeDefinition.BaseTypes)
				{
					if (TypeSystemServices.GetType(baseType).Resolve(targetList, name, flags))
					{
						found = true;
					}
				}
					
				if (IsInterface)
				{
					// also look in System.Object
					if (_typeSystemServices.ObjectType.Resolve(targetList, name, flags))
					{
						found = true;
					}
				}
			}
			
			return found;
		}
		
		public virtual IType BaseType
		{
			get
			{
				return null;
			}
		}
		
		public TypeDefinition TypeDefinition
		{
			get
			{
				return _typeDefinition;
			}
		}
		
		public IType Type
		{
			get
			{
				return this;
			}
		}
		
		public bool IsByRef
		{
			get
			{
				return false;
			}
		}
		
		public bool IsClass
		{
			get
			{
				return NodeType.ClassDefinition == _typeDefinition.NodeType;
			}
		}
		
		public bool IsInterface
		{
			get
			{
				return NodeType.InterfaceDefinition == _typeDefinition.NodeType;
			}
		}
		
		public bool IsEnum
		{
			get
			{
				return NodeType.EnumDefinition == _typeDefinition.NodeType;
			}
		}
		
		public bool IsValueType
		{
			get
			{
				return IsEnum;
			}
		}
		
		public bool IsArray
		{
			get
			{
				return false;
			}
		}
		
		public virtual int GetTypeDepth()
		{
			return 1;
		}
		
		public IEntity GetDefaultMember()
		{
			IType defaultMemberAttribute = _typeSystemServices.Map(typeof(System.Reflection.DefaultMemberAttribute));
			foreach (Boo.Lang.Compiler.Ast.Attribute attribute in _typeDefinition.Attributes)
			{
				IConstructor tag = TypeSystemServices.GetEntity(attribute) as IConstructor;
				if (null != tag)
				{
					if (defaultMemberAttribute == tag.DeclaringType)
					{
						StringLiteralExpression memberName = attribute.Arguments[0] as StringLiteralExpression;
						if (null != memberName)
						{
							_buffer.Clear();
							Resolve(_buffer, memberName.Value, EntityType.Any);
							return NameResolutionService.GetEntityFromList(_buffer);
						}
					}
				}
			}
			return null;
		}
		
		public virtual EntityType EntityType
		{
			get
			{
				return EntityType.Type;
			}
		}
		
		public virtual bool IsSubclassOf(IType other)
		{
			return false;
		}
		
		public virtual bool IsAssignableFrom(IType other)
		{
			return this == other ||
					(!this.IsValueType && Null.Default == other) ||
					other.IsSubclassOf(this);
		}
		
		public virtual IConstructor[] GetConstructors()
		{
			return new IConstructor[0];
		}
		
		public IType[] GetInterfaces()
		{
			if (null == _interfaces)
			{
				_buffer.Clear();
				
				foreach (TypeReference baseType in _typeDefinition.BaseTypes)
				{
					IType tag = TypeSystemServices.GetType(baseType);
					if (tag.IsInterface)
					{
						_buffer.AddUnique(tag);
					}
				}
				
				_interfaces = (IType[])_buffer.ToArray(typeof(IType));
			}
			return _interfaces;
		}
		
		public virtual IEntity[] GetMembers()
		{
			if (null == _members)
			{
				_buffer.Clear();
				foreach (TypeMember member in _typeDefinition.Members)
				{
					IEntity tag = TypeSystemServices.GetEntity(member);
					_buffer.Add(tag);
				}

				_members = (IEntity[])_buffer.ToArray(typeof(IEntity));
				_buffer.Clear();				
			}
			return _members;
		}
		
		override public string ToString()
		{
			return FullName;
		}
	}

}
