using System;

namespace Boo.Ast.Compilation.NameBinding
{
	interface INameSpace
	{
		INameSpace Parent
		{
			get;
		}
		
		INameInfo[] Resolve(string name);
	}
	
	class TypeNameSpace : INameSpace
	{
		const BindingFlags DefaultBindingFlags = BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Static|BindingFlags.Instance;
		
		INameSpace _parent;
		
		Type _type;
		
		public TypeNameSpace(INameSpace parent, Type type)
		{
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
		
		public INameInfo[] Resolve(string name)
		{
			MemberInfo[] members = _type.GetMember(name, DefaultBindingFlags);
			if (members.Length > 0)
			{
				return Wrap(members);
			}
			
			if (null != _parent)
			{
				return _parent.Resolve(name);
			}
			
			return null;
		}
	}
	
	class TypeDefinitionNameResolver : INameResolver
	{
		INameResolver _parent;
		
		TypeDefinition _typeDefinition;
		
		TypeManager _typeManager;
		
		public TypeDefinitionNameResolver(TypeManager typeManager, INameResolver parent, TypeDefinition typeDefinition)
		{
			_typeManager = typeManager;
			_parent = parent;
			_typeDefinition = typeDefinition;
		}
		
		public INameResolver Parent
		{
			get
			{
				return _parent;
			}
		}
		
		public MemberInfo[] Resolve(string name)
		{			
			foreach (TypeMember member in _typeDefinition.Members)
			{
				if (name == member.Name)
				{
					return new MemberInfo[] { _typeManager.GetMemberInfo(member) };
				}
			}			
			if (null != _parent)
			{
				return _parent.Resolve(name);
			}
			return new MemberInfo[0];
		}
	}


}
