#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
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

namespace Boo.Lang.Compiler.TypeSystem
{
	public abstract class AbstractType : IType, INamespace
	{	
		public abstract string Name
		{
			get;
		}
		
		public abstract EntityType EntityType
		{
			get;
		}
		
		public virtual string FullName
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
		
		public virtual bool IsEnum
		{
			get
			{
				return false;
			}
		}
		
		public virtual bool IsValueType
		{
			get
			{
				return false;
			}
		}
		
		public virtual bool IsArray
		{
			get
			{
				return false;
			}
		}
		
		public virtual IType BaseType
		{
			get
			{
				return null;
			}
		}
		
		public virtual IEntity GetDefaultMember()
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
		
		public virtual IConstructor[] GetConstructors()
		{
			return new IConstructor[0];
		}
		
		public virtual IType[] GetInterfaces()
		{
			return new IType[0];
		}
		
		public virtual IEntity[] GetMembers()
		{
			return new IEntity[0];
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
		
		override public EntityType EntityType
		{
			get
			{
				return EntityType.Null;
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
		
		override public EntityType EntityType
		{
			get
			{
				return EntityType.Unknown;
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
		
		override public EntityType EntityType
		{
			get
			{
				return EntityType.Error;
			}
		}
	}
}
