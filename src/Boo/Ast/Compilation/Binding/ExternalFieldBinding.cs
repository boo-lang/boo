namespace Boo.Ast.Compilation.Binding
{
	public class ExternalFieldBinding : IFieldBinding
	{
		BindingManager _bindingManager;
		
		System.Reflection.FieldInfo _field;
		
		public ExternalFieldBinding(BindingManager bindingManager, System.Reflection.FieldInfo field)
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
				return _bindingManager.ToTypeBinding(_field.FieldType);
			}
		}
		
		public System.Type Type
		{
			get
			{
				return _field.FieldType;
			}
		}
		
		public System.Reflection.FieldInfo FieldInfo
		{
			get
			{
				return _field;
			}
		}
	}
}
