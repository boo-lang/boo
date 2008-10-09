using System;
using System.Collections.Generic;
using System.Text;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem
{
	public abstract class AbstractGenericParameter : IGenericParameter
	{
		TypeSystemServices _tss;

		protected AbstractGenericParameter(TypeSystemServices tss)
		{
			_tss = tss;
		}

		abstract public int GenericParameterPosition { get; }

		abstract public bool MustHaveDefaultConstructor { get; }

		abstract public Variance Variance { get; }

		abstract public bool IsClass { get; }

		abstract public bool IsValueType { get; }

		abstract public IType[] GetTypeConstraints();

		abstract public IEntity DeclaringEntity { get; }
		
		public IType DeclaringType
		{
			get
			{
				if (DeclaringEntity is IType)
				{
					return (IType)DeclaringEntity;
				}
				
				if (DeclaringEntity is IMethod)
				{
					return ((IMethod)DeclaringEntity).DeclaringType;
				}

				return null;
			}
		}
		
		public IMethod DeclaringMethod 
		{
			get
			{
				return DeclaringEntity as IMethod;
			}
		}

		bool IType.IsAbstract
		{
			get { return false; }
		}
		
		bool IType.IsInterface
		{
			get { return false; }
		}
		
		bool IType.IsEnum
		{
			get { return false; }
		}
		
		public bool IsByRef
		{
			get { return false; }
		}
		
		bool IType.IsFinal
		{
			get { return true; }
		}
		
		bool IType.IsArray
		{
			get { return false; }
		}
		
		public int GetTypeDepth()
		{
			return DeclaringType.GetTypeDepth() + 1;			
		}
		
		IType IType.GetElementType()
		{
			return null;
		}

		public IType BaseType
		{
			get { return FindBaseType(); }
		}
		
		public IEntity GetDefaultMember()
		{
			return null;
		}
		
		public IConstructor[] GetConstructors()
		{
			if (MustHaveDefaultConstructor)
			{
				// TODO: return a something implementing IConstructor...?
			}
			return null;
		}
		
		public IType[] GetInterfaces()
		{
			return Array.FindAll(GetTypeConstraints(), delegate(IType type) { return type.IsInterface; });
		}

		public bool IsSubclassOf(IType other)
		{
			return (other == BaseType || BaseType.IsSubclassOf(other));
		}
		
		public bool IsAssignableFrom(IType other)
		{
			if (other == this)
			{
				return true;
			}
		
			if (other == Null.Default)
			{
				return IsClass;
			}

			IGenericParameter otherParameter = other as IGenericParameter;
			if (otherParameter != null && Array.Exists(otherParameter.GetTypeConstraints(), IsAssignableFrom))
			{
				return true;
			}

			return false;
		}
		
		IGenericTypeInfo IType.GenericInfo 
		{ 
			get { return null; } 
		}
		
		IConstructedTypeInfo IType.ConstructedInfo 
		{ 
			get { return null; } 
		}

		abstract public string Name { get; }

		public string FullName 
		{
			get 
			{
				return string.Format("{0}.{1}", DeclaringEntity.FullName, Name);
			}
		}
		
		public EntityType EntityType
		{
			get { return EntityType.Type; }
		}
		
		public IType Type
		{
			get { return this; }
		}
		
		INamespace INamespace.ParentNamespace
		{
			get { return (INamespace)DeclaringEntity; }
		}
		
		IEntity[] INamespace.GetMembers()
		{
			return NullNamespace.EmptyEntityArray;
		}
		
		bool INamespace.Resolve(List targetList, string name, EntityType flags)
		{
			bool resolved = false;
			
			foreach (IType type in GetTypeConstraints())
			{
				resolved |= type.Resolve(targetList, name, flags);
			}
			
			resolved |= _tss.ObjectType.Resolve(targetList, name, flags);

			return resolved;
		}
		
		override public string ToString()
		{
			return Name;
		}

		bool IEntityWithAttributes.IsDefined(IType attributeType)
		{
			throw new NotImplementedException();
		}

		protected IType FindBaseType()
		{
			foreach (IType type in GetTypeConstraints())
			{
				if (!type.IsInterface)
				{
					return type;
				}
			}
			return _tss.ObjectType;
		}
	}
}
