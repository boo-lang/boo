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

using System;
using System.Reflection;

namespace Boo.Lang.Compiler.Bindings
{
	public class ExternalTypeBinding : NamespaceBindingCache, ITypeBinding
	{
		const BindingFlags DefaultBindingFlags = BindingFlags.Public |
												BindingFlags.NonPublic |
												BindingFlags.Static |
												BindingFlags.Instance;
		
		BindingManager _bindingManager;
		
		Type _type;
		
		IConstructorBinding[] _constructors;
		
		IBinding[] _members;
		
		ITypeBinding _elementType;
		
		int _typeDepth = -1;
		
		internal ExternalTypeBinding(BindingManager manager, Type type)
		{
			if (null == type)
			{
				throw new ArgumentException("type");
			}
			_bindingManager = manager;
			_type = type;
			if (_type.IsArray)
			{
				_elementType = _bindingManager.AsTypeBinding(type.GetElementType());
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
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Type;
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
		
		public ITypeBinding GetElementType()
		{
			return _elementType;
		}
		
		public ITypeBinding BaseType
		{
			get
			{
				return _bindingManager.AsTypeBinding(_type.BaseType);
			}
		}
		
		public IBinding GetDefaultMember()
		{			
			return _bindingManager.AsBinding(_type.GetDefaultMembers());
		}
		
		public Type Type
		{
			get
			{
				return _type;
			}
		}
		
		public bool IsSubclassOf(ITypeBinding other)
		{
			ExternalTypeBinding external = other as ExternalTypeBinding;
			if (null == external)
			{
				return false;
			}
			
			return _type.IsSubclassOf(external._type) ||
				(external.IsInterface && external._type.IsAssignableFrom(_type))
				;
		}
		
		public bool IsAssignableFrom(ITypeBinding other)
		{
			ExternalTypeBinding external = other as ExternalTypeBinding;
			if (null == external)
			{
				if (BindingType.Null == other.BindingType)
				{
					return !IsValueType;
				}
				return other.IsSubclassOf(this);
			}
			return _type.IsAssignableFrom(external._type);
		}
		
		public IConstructorBinding[] GetConstructors()
		{
			if (null == _constructors)
			{
				BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
				ConstructorInfo[] ctors = _type.GetConstructors(flags);
				_constructors = new IConstructorBinding[ctors.Length];
				for (int i=0; i<_constructors.Length; ++i)
				{
					_constructors[i] = new ExternalConstructorBinding(_bindingManager, ctors[i]);
				}
			}
			return _constructors;
		}
		
		public IBinding[] GetMembers()
		{
			if (null == _members)
			{
				BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
				MemberInfo[] members = _type.GetMembers(flags);
				_members = new IMemberBinding[members.Length];
				for (int i=0; i<members.Length; ++i)
				{
					_members[i] = _bindingManager.AsBinding(members[i]);
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
		
		public virtual IBinding Resolve(string name)
		{						
			bool found;
			IBinding binding = ResolveFromCache(name, out found);
			if (!found)
			{				
				System.Reflection.MemberInfo[] members = _type.GetMember(name, DefaultBindingFlags);
				if (members.Length > 0)
				{				
					binding = _bindingManager.AsBinding(members);
				}
				else if (_type.IsInterface)
				{
					binding = _bindingManager.ObjectTypeBinding.Resolve(name);
				}
				binding = Cache(name, binding);
			}
			return binding;
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
