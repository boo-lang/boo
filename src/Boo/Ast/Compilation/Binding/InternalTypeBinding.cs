using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Ast.Compilation.Binding
{
	public class InternalTypeBinding : ITypeBinding
	{		
		BindingManager _bindingManager;
		TypeDefinition _typeDefinition;
		TypeBuilder _builder;
		
		internal InternalTypeBinding(BindingManager manager, TypeDefinition typeDefinition, TypeBuilder builder)
		{
			_bindingManager = manager;
			_typeDefinition = typeDefinition;
			_builder = builder;
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Type;
			}
		}
		
		public TypeDefinition TypeDefinition
		{
			get
			{
				return _typeDefinition;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return this;
			}
		}
		
		public Type Type
		{
			get
			{
				return _builder;
			}
		}
		
		public TypeBuilder TypeBuilder
		{
			get
			{
				return _builder;
			}
		}
		
		public IConstructorBinding[] GetConstructors()
		{
			return new IConstructorBinding[0];
		}
		
		public IBinding Resolve(string name)
		{			
			foreach (TypeMember member in _typeDefinition.Members)
			{
				if (name == member.Name)
				{
					return _bindingManager.GetBinding(member);
				}
			}
			return null;
		}

	}
}
