using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Ast.Compilation.NameBinding
{
	public class InternalTypeBinding : ITypeBinding
	{		
		BindingManager _manager;
		TypeDefinition _typeDefinition;
		TypeBuilder _builder;
		
		internal InternalTypeBinding(BindingManager manager, TypeDefinition typeDefinition, TypeBuilder builder)
		{
			_manager = manager;
			_typeDefinition = typeDefinition;
			_builder = builder;
		}
		
		public NameBindingType BindingType
		{
			get
			{
				return NameBindingType.Type;
			}
		}
		
		public TypeDefinition TypeDefinition
		{
			get
			{
				return _typeDefinition;
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
	}
}
