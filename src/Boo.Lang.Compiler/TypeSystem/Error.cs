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
