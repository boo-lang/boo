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
	using System.Reflection;

	public class ExternalType : NamespaceCache, IType
	{
		const BindingFlags DefaultBindingFlags = BindingFlags.Public |
												BindingFlags.NonPublic |
												BindingFlags.Static |
												BindingFlags.Instance;
		
		TagService _tagService;
		
		Type _type;
		
		IConstructor[] _constructors;
		
		IType[] _interfaces;
		
		IElement[] _members;
		
		IType _elementType;
		
		int _typeDepth = -1;
		
		internal ExternalType(TagService manager, Type type)
		{
			if (null == type)
			{
				throw new ArgumentException("type");
			}
			_tagService = manager;
			_type = type;
			if (_type.IsArray)
			{
				_elementType = _tagService.Map(type.GetElementType());
			}
		}
		
		public string FullName
		{
			get
			{
				return _type.FullName;
			}
		}
		
		public string Name
		{
			get
			{
				return _type.Name;
			}
		}
		
		public ElementType ElementType
		{
			get
			{
				return ElementType.Type;
			}
		}
		
		public IType BoundType
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
				return _type.IsClass;
			}
		}
		
		public bool IsInterface
		{
			get
			{
				return _type.IsInterface;
			}
		}
		
		public bool IsEnum
		{
			get
			{
				return _type.IsEnum;
			}			
		}
		
		public bool IsValueType
		{
			get
			{
				return _type.IsValueType;
			}
		}
		
		public bool IsArray
		{
			get
			{
				return _type.IsArray;
			}
		}
		
		public int GetArrayRank()
		{
			return _type.GetArrayRank();
		}		
		
		public IType GetElementType()
		{
			return _elementType;
		}
		
		public IType BaseType
		{
			get
			{
				return _tagService.Map(_type.BaseType);
			}
		}
		
		public IElement GetDefaultMember()
		{			
			return _tagService.Map(_type.GetDefaultMembers());
		}
		
		public Type Type
		{
			get
			{
				return _type;
			}
		}
		
		public bool IsSubclassOf(IType other)
		{
			ExternalType external = other as ExternalType;
			if (null == external)
			{
				return false;
			}
			
			return _type.IsSubclassOf(external._type) ||
				(external.IsInterface && external._type.IsAssignableFrom(_type))
				;
		}
		
		public bool IsAssignableFrom(IType other)
		{
			ExternalType external = other as ExternalType;
			if (null == external)
			{
				if (ElementType.Null == other.ElementType)
				{
					return !IsValueType;
				}
				return other.IsSubclassOf(this);
			}
			return _type.IsAssignableFrom(external._type);
		}
		
		public IConstructor[] GetConstructors()
		{
			if (null == _constructors)
			{
				BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
				ConstructorInfo[] ctors = _type.GetConstructors(flags);
				_constructors = new IConstructor[ctors.Length];
				for (int i=0; i<_constructors.Length; ++i)
				{
					_constructors[i] = new ExternalConstructor(_tagService, ctors[i]);
				}
			}
			return _constructors;
		}
		
		public IType[] GetInterfaces()
		{
			if (null == _interfaces)
			{
				Type[] interfaces = _type.GetInterfaces();
				_interfaces = new IType[interfaces.Length];
				for (int i=0; i<_interfaces.Length; ++i)
				{
					_interfaces[i] = _tagService.AsTypeInfo(interfaces[i]);
				}
			}
			return _interfaces;
		}
		
		public IElement[] GetMembers()
		{
			if (null == _members)
			{
				BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
				MemberInfo[] members = _type.GetMembers(flags);
				_members = new IMember[members.Length];
				for (int i=0; i<members.Length; ++i)
				{
					_members[i] = _tagService.AsInfo(members[i]);
				}
			}
			return _members;
		}
		
		public int GetTypeDepth()
		{
			if (-1 == _typeDepth)
			{
				_typeDepth = GetTypeDepth(_type);
			}
			return _typeDepth;
		}
		
		public virtual INamespace ParentNamespace
		{
			get
			{
				return null;
			}
		}
		
		public virtual IElement Resolve(string name)
		{						
			bool found;
			IElement tag = ResolveFromCache(name, out found);
			if (!found)
			{				
				System.Reflection.MemberInfo[] members = _type.GetMember(name, DefaultBindingFlags);
				if (members.Length > 0)
				{				
					tag = _tagService.Map(members);
				}
				else if (_type.IsInterface)
				{
					tag = _tagService.ObjectType.Resolve(name);
				}
				tag = Cache(name, tag);
			}
			return tag;
		}
		
		override public string ToString()
		{
			return FullName;
		}
		
		static int GetTypeDepth(Type type)
		{
			if (type.IsInterface)
			{
				return GetInterfaceDepth(type);
			}
			return GetClassDepth(type);
		}
		
		static int GetClassDepth(Type type)
		{			
			int depth = 0;			
			Type objectType = Types.Object;
			while (type != objectType)
			{
				type = type.BaseType;
				++depth;
			}
			return depth;
		}
		
		static int GetInterfaceDepth(Type type)
		{
			Type[] interfaces = type.GetInterfaces();
			if (interfaces.Length > 0)
			{			
				int current = 0;
				foreach (Type i in interfaces)
				{
					int depth = GetInterfaceDepth(i);
					if (depth > current)
					{
						current = depth;
					}
				}
				return 1+current;
			}
			return 1;
		}
	}
}
