namespace Boo.Ast.Compilation.Binding
{
	public class ParameterBinding : ITypedBinding
	{
		ParameterDeclaration _parameter;
		
		ITypeBinding _type;
		
		int _index;
		
		public ParameterBinding(ParameterDeclaration parameter, ITypeBinding type, int index)
		{
			_parameter = parameter;
			_type = type;
			_index = index;
		}
		
		public string Name
		{
			get
			{
				return _parameter.Name;
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Parameter;
			}
		}
		
		public ParameterDeclaration Parameter
		{
			get
			{
				return _parameter;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return _type;
			}
		}
		
		public System.Type Type
		{
			get
			{
				return _type.Type;
			}
		}
		
		public int Index
		{
			get
			{
				return _index;
			}
		}
	}
}
