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
	
	public class ArrayType : IType, INamespace
	{	
		TagService _tagService;
		
		IType _elementType;
		
		IType _array;
		
		public ArrayType(TagService tagManager, IType elementType)
		{
			_tagService = tagManager;
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
		
		public ElementType ElementType
		{
			get			
			{
				return ElementType.Array;
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
		
		public IElement GetDefaultMember()
		{
			return null;
		}
		
		public virtual bool IsSubclassOf(IType other)
		{
			return other.IsAssignableFrom(_array);
		}
		
		public virtual bool IsAssignableFrom(IType other)
		{			
			if (other == this)
			{
				return true;
			}
			
			if (other.IsArray)
			{
				IType otherElementType = other.GetElementType();
				if (_elementType.IsValueType || otherElementType.IsValueType)
				{
					return _elementType == otherElementType;
				}
				return _elementType.IsAssignableFrom(otherElementType);
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
		
		public IElement[] GetMembers()
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
		
		public IElement Resolve(string name)
		{
			return _array.Resolve(name);
		}
		
		override public string ToString()
		{
			return Name;
		}
	}
}
