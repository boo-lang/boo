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

namespace Boo.Lang.Compiler.Bindings
{
	using System;
	using System.Collections;
	using Boo.Lang.Compiler.Services;
	using Boo.Lang.Compiler.Ast;
	
	public abstract class AbstractInternalBinding : IInternalBinding
	{
		protected bool _visited;
		
		public AbstractInternalBinding()
		{
			_visited = false;
		}
		
		public AbstractInternalBinding(bool visited)
		{
			_visited = visited;
		}
		
		public abstract Node Node
		{
			get;
		}
		
		public bool Visited
		{
			get
			{
				return _visited;
			}
			
			set
			{
				_visited = value;
			}
		}		
	}
	
	public abstract class AbstractInternalTypeBinding : AbstractInternalBinding, ITypeBinding, INamespace
	{		
		protected DefaultBindingService _bindingService;
		
		protected TypeDefinition _typeDefinition;
		
		protected IBinding[] _members;
		
		protected ITypeBinding[] _interfaces;
		
		protected INamespace _parentNamespace;
		
		protected List _buffer = new List();
		
		protected AbstractInternalTypeBinding(DefaultBindingService bindingManager, TypeDefinition typeDefinition)
		{
			_bindingService = bindingManager;
			_typeDefinition = typeDefinition;
			_parentNamespace = (INamespace)DefaultBindingService.GetBinding(_typeDefinition.ParentNode);
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
		
		override public Node Node
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
		
		public virtual IBinding Resolve(string name)
		{			
			_buffer.Clear();			
			
			foreach (IBinding binding in GetMembers())
			{
				if (binding.Name == name)
				{
					_buffer.Add(binding);
				}
			}
			
			if (0 == _buffer.Count)
			{
				foreach (TypeReference baseType in _typeDefinition.BaseTypes)
				{
					IBinding binding = _bindingService.GetBoundType(baseType).Resolve(name);
					if (null != binding)
					{
						_buffer.AddUnique(binding);
					}
				}
				
				if (IsInterface)
				{
					// also look in System.Object
					IBinding binding = _bindingService.ObjectTypeBinding.Resolve(name);
					if (null != binding)
					{
						_buffer.AddUnique(binding);						
					}
				}
			}
			
			if (_buffer.Count > 0)
			{
				if (_buffer.Count > 1)
				{
					return new AmbiguousBinding((IBinding[])_buffer.ToArray(typeof(IBinding)));
				}
				else
				{
					return (IBinding)_buffer[0];
				}
			}
			return null;
		}
		
		IBinding CreateCorrectBinding(TypeMember member)
		{
			switch (member.NodeType)
			{
				case NodeType.Method:
				{
					return new InternalMethodBinding(_bindingService, (Method)member);
				}
				
				case NodeType.Constructor:
				{
					return new InternalConstructorBinding(_bindingService, (Constructor)member);
				}
				
				case NodeType.Field:
				{
					return new InternalFieldBinding(_bindingService, (Field)member);
				}
				
				case NodeType.EnumDefinition:
				{
					return new EnumTypeBinding(_bindingService, (EnumDefinition)member);
				}
				
				case NodeType.EnumMember:
				{
					return new InternalEnumMemberBinding(_bindingService, (EnumMember)member);
				}
				
				case NodeType.Property:
				{
					return new InternalPropertyBinding(_bindingService, (Property)member);
				}
			}
			throw new NotImplementedException(member.GetType().ToString());
		}
		
		public virtual ITypeBinding BaseType
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
		
		public ITypeBinding BoundType
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
		
		public ITypeBinding GetElementType()
		{
			return null;
		}
		
		public IBinding GetDefaultMember()
		{
			ITypeBinding defaultMemberAttribute = _bindingService.AsTypeBinding(typeof(System.Reflection.DefaultMemberAttribute));
			foreach (Boo.Lang.Compiler.Ast.Attribute attribute in _typeDefinition.Attributes)
			{
				IConstructorBinding binding = DefaultBindingService.GetBinding(attribute) as IConstructorBinding;
				if (null != binding)
				{
					if (defaultMemberAttribute == binding.DeclaringType)
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
		
		public virtual BindingType BindingType
		{
			get
			{
				return BindingType.Type;
			}
		}
		
		public virtual bool IsSubclassOf(ITypeBinding other)
		{
			return false;
		}
		
		public virtual bool IsAssignableFrom(ITypeBinding other)
		{
			return this == other ||
					(!this.IsValueType && NullBinding.Default == other) ||
					other.IsSubclassOf(this);
		}
		
		public virtual IConstructorBinding[] GetConstructors()
		{
			return new IConstructorBinding[0];
		}
		
		public ITypeBinding[] GetInterfaces()
		{
			if (null == _interfaces)
			{
				_buffer.Clear();
				
				foreach (TypeReference baseType in _typeDefinition.BaseTypes)
				{
					ITypeBinding binding = (ITypeBinding)_bindingService.GetBoundType(baseType);
					if (binding.IsInterface)
					{
						_buffer.AddUnique(binding);
					}
				}
				
				_interfaces = (ITypeBinding[])_buffer.ToArray(typeof(ITypeBinding));
			}
			return _interfaces;
		}
		
		public virtual IBinding[] GetMembers()
		{
			if (null == _members)
			{
				_buffer.Clear();
				foreach (TypeMember member in _typeDefinition.Members)
				{
					IBinding binding = member.Binding;
					if (null == binding)
					{						
						binding = CreateCorrectBinding(member);
						DefaultBindingService.Bind(member, binding);
					}	
					
					if (BindingType.Type == binding.BindingType)
					{
						binding = _bindingService.AsTypeReference((ITypeBinding)binding);
					}
					_buffer.Add(binding);
				}

				_members = (IBinding[])_buffer.ToArray(typeof(IBinding));
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
