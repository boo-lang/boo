namespace Boo.Ast.Compilation.Binding
{
	using System;
	using Boo.Ast;
	
	public class InternalFieldBinding : IFieldBinding
	{
		BindingManager _bindingManager;
		Field _field;
		
		public InternalFieldBinding(BindingManager bindingManager, Field field)
		{
			_bindingManager = bindingManager;
			_field = field;
		}
		
		public string Name
		{
			get
			{
				return _field.Name;
			}
		}
		
		public bool IsStatic
		{
			get
			{
				return _field.IsStatic;
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Field;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return _bindingManager.GetBoundType(_field.Type);
			}
		}
		
		public ITypeBinding DeclaringType
		{
			get
			{
				return (ITypeBinding)_bindingManager.GetBinding(_field.ParentNode);
			}
		}
		
		public Field Field
		{
			get
			{
				return _field;
			}
		}
	}
}
