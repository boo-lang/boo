using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Ast.Compilation.Binding
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
