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

namespace Boo.Lang.Compiler.Infos
{
	using System;
	using System.Collections;
	using Boo.Lang.Compiler.Services;
	using Boo.Lang.Compiler.Ast;
	
	public abstract class AbstractInternalInfo : IInternalInfo
	{
		protected bool _visited;
		
		public AbstractInternalInfo()
		{
			_visited = false;
		}
		
		public AbstractInternalInfo(bool visited)
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
	
	public abstract class AbstractInternalTypeInfo : AbstractInternalInfo, ITypeInfo, INamespace
	{		
		protected DefaultInfoService _bindingService;
		
		protected TypeDefinition _typeDefinition;
		
		protected IInfo[] _members;
		
		protected ITypeInfo[] _interfaces;
		
		protected INamespace _parentNamespace;
		
		protected List _buffer = new List();
		
		protected AbstractInternalTypeInfo(DefaultInfoService bindingManager, TypeDefinition typeDefinition)
		{
			_bindingService = bindingManager;
			_typeDefinition = typeDefinition;
			_parentNamespace = (INamespace)DefaultInfoService.GetInfo(_typeDefinition.ParentNode);
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
		
		public virtual IInfo Resolve(string name)
		{			
			_buffer.Clear();			
			
			foreach (IInfo binding in GetMembers())
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
					IInfo binding = _bindingService.GetBoundType(baseType).Resolve(name);
					if (null != binding)
					{
						_buffer.AddUnique(binding);
					}
				}
				
				if (IsInterface)
				{
					// also look in System.Object
					IInfo binding = _bindingService.ObjectTypeInfo.Resolve(name);
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
					return new AmbiguousInfo((IInfo[])_buffer.ToArray(typeof(IInfo)));
				}
				else
				{
					return (IInfo)_buffer[0];
				}
			}
			return null;
		}
		
		IInfo CreateCorrectInfo(TypeMember member)
		{
			switch (member.NodeType)
			{
				case NodeType.Method:
				{
					return new InternalMethodInfo(_bindingService, (Method)member);
				}
				
				case NodeType.Constructor:
				{
					return new InternalConstructorInfo(_bindingService, (Constructor)member);
				}
				
				case NodeType.Field:
				{
					return new InternalFieldInfo(_bindingService, (Field)member);
				}
				
				case NodeType.EnumDefinition:
				{
					return new EnumTypeInfo(_bindingService, (EnumDefinition)member);
				}
				
				case NodeType.EnumMember:
				{
					return new InternalEnumMemberInfo(_bindingService, (EnumMember)member);
				}
				
				case NodeType.Property:
				{
					return new InternalPropertyInfo(_bindingService, (Property)member);
				}
			}
			throw new NotImplementedException(member.GetType().ToString());
		}
		
		public virtual ITypeInfo BaseType
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
		
		public ITypeInfo BoundType
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
		
		public ITypeInfo GetElementType()
		{
			return null;
		}
		
		public IInfo GetDefaultMember()
		{
			ITypeInfo defaultMemberAttribute = _bindingService.AsTypeInfo(typeof(System.Reflection.DefaultMemberAttribute));
			foreach (Boo.Lang.Compiler.Ast.Attribute attribute in _typeDefinition.Attributes)
			{
				IConstructorInfo binding = DefaultInfoService.GetInfo(attribute) as IConstructorInfo;
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
		
		public virtual InfoType InfoType
		{
			get
			{
				return InfoType.Type;
			}
		}
		
		public virtual bool IsSubclassOf(ITypeInfo other)
		{
			return false;
		}
		
		public virtual bool IsAssignableFrom(ITypeInfo other)
		{
			return this == other ||
					(!this.IsValueType && NullInfo.Default == other) ||
					other.IsSubclassOf(this);
		}
		
		public virtual IConstructorInfo[] GetConstructors()
		{
			return new IConstructorInfo[0];
		}
		
		public ITypeInfo[] GetInterfaces()
		{
			if (null == _interfaces)
			{
				_buffer.Clear();
				
				foreach (TypeReference baseType in _typeDefinition.BaseTypes)
				{
					ITypeInfo binding = (ITypeInfo)_bindingService.GetBoundType(baseType);
					if (binding.IsInterface)
					{
						_buffer.AddUnique(binding);
					}
				}
				
				_interfaces = (ITypeInfo[])_buffer.ToArray(typeof(ITypeInfo));
			}
			return _interfaces;
		}
		
		public virtual IInfo[] GetMembers()
		{
			if (null == _members)
			{
				_buffer.Clear();
				foreach (TypeMember member in _typeDefinition.Members)
				{
					IInfo binding = member.Info;
					if (null == binding)
					{						
						binding = CreateCorrectInfo(member);
						DefaultInfoService.Bind(member, binding);
					}	
					
					if (InfoType.Type == binding.InfoType)
					{
						binding = _bindingService.AsTypeReference((ITypeInfo)binding);
					}
					_buffer.Add(binding);
				}

				_members = (IInfo[])_buffer.ToArray(typeof(IInfo));
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
