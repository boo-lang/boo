namespace Boo.Ast.Compilation.Binding
{
	public class ExternalPropertyBinding : IPropertyBinding
	{
		BindingManager _bindingManager;
		
		System.Reflection.PropertyInfo _property;
		
		public ExternalPropertyBinding(BindingManager bindingManager, System.Reflection.PropertyInfo property)
		{
			_bindingManager = bindingManager;
			_property = property;
		}
		
		public bool IsStatic
		{
			get
			{
				System.Reflection.MethodInfo mi = _property.GetGetMethod();
				if (null != mi)
				{
					return mi.IsStatic;
				}
				return _property.GetSetMethod().IsStatic;
			}
		}
		
		public string Name
		{
			get
			{
				return _property.Name;
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Property;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return _bindingManager.ToTypeBinding(_property.PropertyType);
			}
		}
		
		public System.Type Type
		{
			get
			{
				return _property.PropertyType;
			}
		}
		
		public System.Reflection.PropertyInfo PropertyInfo
		{
			get
			{
				return _property;
			}
		}
	}
}
