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
	public abstract class AbstractTypeBinding : ITypeBinding, INamespace
	{	
		public abstract string Name
		{
			get;
		}
		
		public abstract BindingType BindingType
		{
			get;
		}
		
		public string FullName
		{
			get
			{
				return Name;
			}
		}
		
		public virtual ITypeBinding BoundType
		{
			get
			{
				return this;
			}
		}
		
		public virtual bool IsClass
		{
			get
			{
				return false;
			}
		}
		
		public virtual bool IsInterface
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
				return false;
			}
		}
		
		public int GetArrayRank()
		{
			return 0;
		}		
		
		public ITypeBinding GetElementType()
		{
			return null;
		}
		
		public ITypeBinding BaseType
		{
			get
			{
				return null;
			}
		}
		
		public IBinding GetDefaultMember()
		{
			return null;
		}
		
		public virtual int GetTypeDepth()
		{
			return 0;
		}
		
		public virtual bool IsSubclassOf(ITypeBinding other)
		{
			return false;
		}
		
		public virtual bool IsAssignableFrom(ITypeBinding other)
		{
			return false;
		}
		
		public IConstructorBinding[] GetConstructors()
		{
			return new IConstructorBinding[0];
		}
		
		public IBinding[] GetMembers()
		{
			return new IBinding[0];
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return null;
			}
		}
		
		public IBinding Resolve(string name)
		{
			return null;
		}
		
		override public string ToString()
		{
			return Name;
		}
	}
	
	public class NullBinding : AbstractTypeBinding
	{
		public static NullBinding Default = new NullBinding();
		
		private NullBinding()
		{
		}
		
		override public string Name
		{
			get
			{
				return "null";
			}
		}
		
		override public BindingType BindingType
		{
			get
			{
				return BindingType.Null;
			}
		}
	}
	
	public class UnknownBinding : AbstractTypeBinding
	{
		public static UnknownBinding Default = new UnknownBinding();
		
		private UnknownBinding()
		{
		}
		
		override public string Name
		{
			get
			{
				return "unknown";
			}
		}
		
		override public BindingType BindingType
		{
			get
			{
				return BindingType.Unknown;
			}
		}
	}
	
	public class ErrorBinding : AbstractTypeBinding
	{
		public static ErrorBinding Default = new ErrorBinding();
		
		private ErrorBinding()
		{			
		}	
		
		override public string Name
		{
			get
			{
				return "error";
			}
		}
		
		override public BindingType BindingType
		{
			get
			{
				return BindingType.Error;
			}
		}
	}
}
