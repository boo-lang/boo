#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
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
			DefaultMemberAttribute attribute = (DefaultMemberAttribute)Attribute.GetCustomAttribute(_type, typeof(DefaultMemberAttribute));
			if (null != attribute)
			{
				return Resolve(attribute.MemberName);
			}
			return null;
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
				throw new NotImplementedException(other.ToString());
			}
			return _type.IsSubclassOf(external._type);
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
	}
}
