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
	using System.Reflection;
	using Boo.Lang.Compiler.Services;

	public class ExternalTypeInfo : NamespaceInfoCache, ITypeInfo
	{
		const BindingFlags DefaultBindingFlags = BindingFlags.Public |
												BindingFlags.NonPublic |
												BindingFlags.Static |
												BindingFlags.Instance;
		
		DefaultInfoService _bindingService;
		
		Type _type;
		
		IConstructorInfo[] _constructors;
		
		ITypeInfo[] _interfaces;
		
		IInfo[] _members;
		
		ITypeInfo _elementType;
		
		int _typeDepth = -1;
		
		internal ExternalTypeInfo(DefaultInfoService manager, Type type)
		{
			if (null == type)
			{
				throw new ArgumentException("type");
			}
			_bindingService = manager;
			_type = type;
			if (_type.IsArray)
			{
				_elementType = _bindingService.AsTypeInfo(type.GetElementType());
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
		
		public InfoType InfoType
		{
			get
			{
				return InfoType.Type;
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
		
		public ITypeInfo GetElementType()
		{
			return _elementType;
		}
		
		public ITypeInfo BaseType
		{
			get
			{
				return _bindingService.AsTypeInfo(_type.BaseType);
			}
		}
		
		public IInfo GetDefaultMember()
		{			
			return _bindingService.AsInfo(_type.GetDefaultMembers());
		}
		
		public Type Type
		{
			get
			{
				return _type;
			}
		}
		
		public bool IsSubclassOf(ITypeInfo other)
		{
			ExternalTypeInfo external = other as ExternalTypeInfo;
			if (null == external)
			{
				return false;
			}
			
			return _type.IsSubclassOf(external._type) ||
				(external.IsInterface && external._type.IsAssignableFrom(_type))
				;
		}
		
		public bool IsAssignableFrom(ITypeInfo other)
		{
			ExternalTypeInfo external = other as ExternalTypeInfo;
			if (null == external)
			{
				if (InfoType.Null == other.InfoType)
				{
					return !IsValueType;
				}
				return other.IsSubclassOf(this);
			}
			return _type.IsAssignableFrom(external._type);
		}
		
		public IConstructorInfo[] GetConstructors()
		{
			if (null == _constructors)
			{
				BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
				ConstructorInfo[] ctors = _type.GetConstructors(flags);
				_constructors = new IConstructorInfo[ctors.Length];
				for (int i=0; i<_constructors.Length; ++i)
				{
					_constructors[i] = new ExternalConstructorInfo(_bindingService, ctors[i]);
				}
			}
			return _constructors;
		}
		
		public ITypeInfo[] GetInterfaces()
		{
			if (null == _interfaces)
			{
				Type[] interfaces = _type.GetInterfaces();
				_interfaces = new ITypeInfo[interfaces.Length];
				for (int i=0; i<_interfaces.Length; ++i)
				{
					_interfaces[i] = _bindingService.AsTypeInfo(interfaces[i]);
				}
			}
			return _interfaces;
		}
		
		public IInfo[] GetMembers()
		{
			if (null == _members)
			{
				BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
				MemberInfo[] members = _type.GetMembers(flags);
				_members = new IMemberInfo[members.Length];
				for (int i=0; i<members.Length; ++i)
				{
					_members[i] = _bindingService.AsInfo(members[i]);
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
		
		public virtual IInfo Resolve(string name)
		{						
			bool found;
			IInfo binding = ResolveFromCache(name, out found);
			if (!found)
			{				
				System.Reflection.MemberInfo[] members = _type.GetMember(name, DefaultBindingFlags);
				if (members.Length > 0)
				{				
					binding = _bindingService.AsInfo(members);
				}
				else if (_type.IsInterface)
				{
					binding = _bindingService.ObjectTypeInfo.Resolve(name);
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
