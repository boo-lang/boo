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
	public abstract class AbstractType : IType, INamespace
	{	
		public abstract string Name
		{
			get;
		}
		
		public abstract ElementType ElementType
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
		
		public virtual IType Type
		{
			get
			{
				return this;
			}
		}
		
		public virtual bool IsByRef
		{
			get
			{
				return false;
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
		
		public IType GetElementType()
		{
			return null;
		}
		
		public IType BaseType
		{
			get
			{
				return null;
			}
		}
		
		public IElement GetDefaultMember()
		{
			return null;
		}
		
		public virtual int GetTypeDepth()
		{
			return 0;
		}
		
		public virtual bool IsSubclassOf(IType other)
		{
			return false;
		}
		
		public virtual bool IsAssignableFrom(IType other)
		{
			return false;
		}
		
		public IConstructor[] GetConstructors()
		{
			return new IConstructor[0];
		}
		
		public IType[] GetInterfaces()
		{
			return new IType[0];
		}
		
		public IElement[] GetMembers()
		{
			return new IElement[0];
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return null;
			}
		}
		
		public bool Resolve(Boo.Lang.List targetList, string name, ElementType flags)
		{
			return false;
		}
		
		override public string ToString()
		{
			return Name;
		}
	}
	
	public class Null : AbstractType
	{
		public static Null Default = new Null();
		
		private Null()
		{
		}
		
		override public string Name
		{
			get
			{
				return "null";
			}
		}
		
		override public ElementType ElementType
		{
			get
			{
				return ElementType.Null;
			}
		}
	}
	
	public class Unknown : AbstractType
	{
		public static Unknown Default = new Unknown();
		
		private Unknown()
		{
		}
		
		override public string Name
		{
			get
			{
				return "unknown";
			}
		}
		
		override public ElementType ElementType
		{
			get
			{
				return ElementType.Unknown;
			}
		}
	}
	
	public class Error : AbstractType
	{
		public static Error Default = new Error();
		
		private Error()
		{			
		}	
		
		override public string Name
		{
			get
			{
				return "error";
			}
		}
		
		override public ElementType ElementType
		{
			get
			{
				return ElementType.Error;
			}
		}
	}
}
