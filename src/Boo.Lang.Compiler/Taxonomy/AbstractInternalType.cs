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

namespace Boo.Lang.Compiler.Taxonomy
{
	using System;
	using System.Collections;
	using Boo.Lang.Compiler.Ast;
	
	public abstract class AbstractInternalType : IInternalElement, IType, INamespace
	{		
		protected TagService _tagService;
		
		protected TypeDefinition _typeDefinition;
		
		protected IElement[] _members;
		
		protected IType[] _interfaces;
		
		protected INamespace _parentNamespace;
		
		protected List _buffer = new List();
		
		protected AbstractInternalType(TagService tagManager, TypeDefinition typeDefinition)
		{
			_tagService = tagManager;
			_typeDefinition = typeDefinition;
			_parentNamespace = (INamespace)TagService.GetTag(_typeDefinition.ParentNode);
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
				return _parentNamespace;
			}
		}
		
		public virtual IElement Resolve(string name)
		{			
			_buffer.Clear();			
			
			foreach (IElement tag in GetMembers())
			{
				if (tag.Name == name)
				{
					_buffer.Add(tag);
				}
			}
			
			foreach (TypeReference baseType in _typeDefinition.BaseTypes)
			{
				IElement tag = _tagService.GetBoundType(baseType).Resolve(name);
				if (null != tag)
				{
					_buffer.AddUnique(tag);
				}
			}
				
			if (IsInterface)
			{
				// also look in System.Object
				IElement tag = _tagService.ObjectType.Resolve(name);
				if (null != tag)
				{
					_buffer.AddUnique(tag);						
				}
			}
			
			if (_buffer.Count > 0)
			{
				if (_buffer.Count > 1)
				{
					return new Ambiguous((IElement[])_buffer.ToArray(typeof(IElement)));
				}
				else
				{
					return (IElement)_buffer[0];
				}
			}
			return null;
		}
		
		IElement CreateCorrectElement(TypeMember member)
		{
			switch (member.NodeType)
			{
				case NodeType.Method:
				{
					return new InternalMethod(_tagService, (Method)member);
				}
				
				case NodeType.Constructor:
				{
					return new InternalConstructor(_tagService, (Constructor)member);
				}
				
				case NodeType.Field:
				{
					return new InternalField(_tagService, (Field)member);
				}
				
				case NodeType.EnumDefinition:
				{
					return new EnumType(_tagService, (EnumDefinition)member);
				}
				
				case NodeType.EnumMember:
				{
					return new InternalEnumMember(_tagService, (EnumMember)member);
				}
				
				case NodeType.Property:
				{
					return new InternalProperty(_tagService, (Property)member);
				}
			}
			throw new NotImplementedException(member.GetType().ToString());
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
		
		public int GetArrayRank()
		{
			return 0;
		}
		
		public IType GetElementType()
		{
			return null;
		}
		
		public IElement GetDefaultMember()
		{
			IType defaultMemberAttribute = _tagService.Map(typeof(System.Reflection.DefaultMemberAttribute));
			foreach (Boo.Lang.Compiler.Ast.Attribute attribute in _typeDefinition.Attributes)
			{
				IConstructor tag = TagService.GetTag(attribute) as IConstructor;
				if (null != tag)
				{
					if (defaultMemberAttribute == tag.DeclaringType)
					{
						StringLiteralExpression memberName = attribute.Arguments[0] as StringLiteralExpression;
						if (null != memberName)
						{
							return Resolve(memberName.Value);
						}
					}
				}
			}
			return null;
		}
		
		public virtual ElementType ElementType
		{
			get
			{
				return ElementType.Type;
			}
		}
		
		public virtual bool IsSubclassOf(IType other)
		{
			return false;
		}
		
		public virtual bool IsAssignableFrom(IType other)
		{
			return this == other ||
					(!this.IsValueType && NullInfo.Default == other) ||
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
					IType tag = (IType)_tagService.GetBoundType(baseType);
					if (tag.IsInterface)
					{
						_buffer.AddUnique(tag);
					}
				}
				
				_interfaces = (IType[])_buffer.ToArray(typeof(IType));
			}
			return _interfaces;
		}
		
		public virtual IElement[] GetMembers()
		{
			if (null == _members)
			{
				_buffer.Clear();
				foreach (TypeMember member in _typeDefinition.Members)
				{
					IElement tag = member.Info;
					if (null == tag)
					{						
						tag = CreateCorrectElement(member);
						member.Tag = tag;
					}	
					
					if (ElementType.Type == tag.ElementType)
					{
						tag = _tagService.GetTypeReference((IType)tag);
					}
					_buffer.Add(tag);
				}

				_members = (IElement[])_buffer.ToArray(typeof(IElement));
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
