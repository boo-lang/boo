#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Reflection;

	public class ExternalType : IType
	{
		protected const BindingFlags DefaultBindingFlags = BindingFlags.Public |
												BindingFlags.NonPublic |
												BindingFlags.Static |
												BindingFlags.Instance;
		
		TypeSystemServices _typeSystemServices;
		
		Type _type;
		
		IConstructor[] _constructors;
		
		IType[] _interfaces;
		
		IEntity[] _members;
		
		IType _elementType;
		
		int _typeDepth = -1;
		
		internal ExternalType(TypeSystemServices manager, Type type)
		{
			if (null == type)
			{
				throw new ArgumentException("type");
			}
			_typeSystemServices = manager;
			_type = type;
			if (_type.IsArray)
			{
				_elementType = _typeSystemServices.Map(type.GetElementType());
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
		
		public EntityType EntityType
		{
			get
			{
				return EntityType.Type;
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
				return _type.IsByRef;
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
				return false;
			}
		}
		
		public IType BaseType
		{
			get
			{
				return _typeSystemServices.Map(_type.BaseType);
			}
		}
		
		public IEntity GetDefaultMember()
		{			
			return _typeSystemServices.Map(_type.GetDefaultMembers());
		}
		
		public Type ActualType
		{
			get
			{
				return _type;
			}
		}
		
		public virtual bool IsSubclassOf(IType other)
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
		
		public virtual bool IsAssignableFrom(IType other)
		{
			ExternalType external = other as ExternalType;
			if (null == external)
			{
				if (EntityType.Null == other.EntityType)
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
					_constructors[i] = new ExternalConstructor(_typeSystemServices, ctors[i]);
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
					_interfaces[i] = _typeSystemServices.Map(interfaces[i]);
				}
			}
			return _interfaces;
		}
		
		public IEntity[] GetMembers()
		{
			if (null == _members)
			{
				BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
				MemberInfo[] members = _type.GetMembers(flags);
				_members = new IEntity[members.Length];
				for (int i=0; i<members.Length; ++i)
				{
					_members[i] = _typeSystemServices.Map(members[i]);
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
		
		public virtual bool Resolve(Boo.Lang.List targetList, string name, EntityType flags)
		{					
			bool found = false;
			foreach (System.Reflection.MemberInfo member in _type.GetMember(name, DefaultBindingFlags))
			{
				targetList.AddUnique(_typeSystemServices.Map(member));
				found = true;
			}
			
			if (IsInterface)
			{				
				if (_typeSystemServices.ObjectType.Resolve(targetList, name, flags))
				{
					found = true;
				}
				
				foreach (IType baseInterface in GetInterfaces())
				{
					found |= baseInterface.Resolve(targetList, name, flags);
				}
			}
			return found;
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
