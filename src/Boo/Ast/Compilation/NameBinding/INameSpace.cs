using System;
using BindingFlags = System.Reflection.BindingFlags;

namespace Boo.Ast.Compilation.NameBinding
{
	public enum NameInfoType
	{
		TypeInfo,
		MethodInfo,
		AmbiguousNameInfo
	}
	
	public interface INameInfo
	{		
		NameInfoType InfoType
		{
			get;
		}
	}
	
	public interface ITypeInfo : INameInfo
	{
		System.Type Type
		{
			get;
		}
	}
	
	public interface IMethodInfo : INameInfo
	{
		int ParameterCount
		{
			get;
		}
		
		System.Reflection.MethodInfo MethodInfo
		{
			get;
		}
		
		ITypeInfo ReturnType
		{
			get;
		}
	}
	
	public interface INameSpace
	{
		INameSpace Parent
		{
			get;
		}
		
		INameInfo Resolve(string name);
	}
	
	class TypeNameSpace : INameSpace
	{
		const BindingFlags DefaultBindingFlags = BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Static|BindingFlags.Instance;
		
		TypeManager _typeManager;
		
		INameSpace _parent;
		
		Type _type;
		
		public TypeNameSpace(TypeManager typeManager, INameSpace parent, Type type)
		{
			_typeManager = typeManager;
			_parent = parent;
			_type = type;
		}		
		
		public INameSpace Parent
		{
			get
			{
				return _parent;
			}
		}
		
		public INameInfo Resolve(string name)
		{
			System.Reflection.MemberInfo[] members = _type.GetMember(name, DefaultBindingFlags);
			if (members.Length > 0)
			{				
				return _typeManager.ToNameInfo(members);
			}
			
			if (null != _parent)
			{
				return _parent.Resolve(name);
			}
			
			return null;
		}
	}
	
	class TypeDefinitionNameSpace : INameSpace
	{
		INameSpace _parent;
		
		TypeDefinition _typeDefinition;
		
		TypeManager _typeManager;
		
		public TypeDefinitionNameSpace(TypeManager typeManager, INameSpace parent, TypeDefinition typeDefinition)
		{
			_typeManager = typeManager;
			_parent = parent;
			_typeDefinition = typeDefinition;
		}
		
		public INameSpace Parent
		{
			get
			{
				return _parent;
			}
		}
		
		public INameInfo Resolve(string name)
		{			
			foreach (TypeMember member in _typeDefinition.Members)
			{
				if (name == member.Name)
				{
					return _typeManager.GetNameInfo(member);
				}
			}			
			if (null != _parent)
			{
				return _parent.Resolve(name);
			}
			return null;
		}
	}


}
