#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	
	public class ArrayType : IArrayType
	{	
		TypeSystemServices _typeSystemServices;
		
		IType _elementType;
		
		IType _array;
		
		public ArrayType(TypeSystemServices tagManager, IType elementType)
		{
			_typeSystemServices = tagManager;
			_array = tagManager.ArrayType;
			_elementType = elementType;
		}
		
		public string Name
		{
			get
			{
				return "(" + _elementType.FullName + ")";
			}
		}
		
		public EntityType EntityType
		{
			get			
			{
				return EntityType.Array;
			}
		}
		
		public string FullName
		{
			get
			{
				return Name;
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
				return false;
			}
		}
		
		public bool IsInterface
		{
			get
			{
				return false;
			}
		}
		
		public bool IsEnum
		{
			get
			{
				return false;
			}
		}
		
		public bool IsValueType
		{
			get
			{
				return false;
			}
		}
		
		public bool IsArray
		{
			get
			{
				return true;
			}
		}
		
		public int GetTypeDepth()
		{
			return 2;
		}
		
		public int GetArrayRank()
		{
			return 1;
		}		
		
		public IType GetElementType()
		{
			return _elementType;
		}
		
		public IType BaseType
		{
			get
			{
				return _array;
			}
		}
		
		public IEntity GetDefaultMember()
		{
			return null;
		}
		
		public virtual bool IsSubclassOf(IType other)
		{
			return other.IsAssignableFrom(_array);
		}
		
		public virtual bool IsAssignableFrom(IType other)
		{			
			if (other == this || other == Null.Default)
			{
				return true;
			}
			
			if (other.IsArray)
			{
				IType otherEntityType = ((IArrayType)other).GetElementType();
				if (_elementType.IsValueType || otherEntityType.IsValueType)
				{
					return _elementType == otherEntityType;
				}
				return _elementType.IsAssignableFrom(otherEntityType);
			}
			return false;
		}
		
		public IConstructor[] GetConstructors()
		{
			return new IConstructor[0];
		}
		
		public IType[] GetInterfaces()
		{
			return null;
		}
		
		public IEntity[] GetMembers()
		{
			return null;
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return _array.ParentNamespace;
			}
		}
		
		public bool Resolve(Boo.Lang.List targetList, string name, EntityType flags)
		{
			return _array.Resolve(targetList, name, flags);
		}
		
		override public string ToString()
		{
			return Name;
		}
	}
}
