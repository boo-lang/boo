using System;
using BindingFlags = System.Reflection.BindingFlags;

namespace Boo.Ast.Compilation.NameBinding
{
	public enum NameInfoType
	{
		Type,
		Method,		
		Local,		
		AmbiguousName
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
	
	public class LocalInfo : INameInfo
	{
		TypeManager _manager;
		
		Local _local;
		
		ITypeInfo _typeInfo;
		
		System.Reflection.Emit.LocalBuilder _builder;
		
		public LocalInfo(TypeManager manager, Local local, ITypeInfo typeInfo)
		{
			_manager = manager;
			_local = local;
			_typeInfo = typeInfo;
		}
		
		public NameInfoType InfoType
		{
			get
			{
				return NameInfoType.Local;
			}
		}
		
		public Local Local
		{
			get
			{
				return _local;
			}
		}
		
		public ITypeInfo TypeInfo
		{
			get
			{
				return _typeInfo;
			}
		}
		
		public Type Type
		{
			get
			{
				return _typeInfo.Type;
			}
		}
		
		public System.Reflection.Emit.LocalBuilder LocalBuilder
		{
			get
			{
				return _builder;
			}
			
			set
			{
				_builder = value;
			}
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
	
	class MethodNameSpace : INameSpace
	{
		INameSpace _parent;
		
		Method _method;
		
		TypeManager _manager;
		
		public MethodNameSpace(TypeManager manager, INameSpace parent, Method method)
		{
			_manager = manager;
			_method = method;
			_parent = parent;
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
			foreach (Local local in _method.Locals)
			{
				if (name == local.Name)
				{
					return _manager.GetNameInfo(local);
				}
			}
			return _parent.Resolve(name);
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
