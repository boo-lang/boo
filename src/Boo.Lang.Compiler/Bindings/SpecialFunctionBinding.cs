namespace Boo.Lang.Compiler.Bindings
{
	public enum SpecialFunction
	{
		Len
	}
	
	public class SpecialFunctionBinding : IBinding
	{
		SpecialFunction _function;
		
		public SpecialFunctionBinding(SpecialFunction f)
		{
			_function = f;
		}
		
		public string Name
		{
			get
			{
				return _function.ToString();
			}
		}
		
		public string FullName
		{
			get
			{
				return Name;
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.SpecialFunction;
			}
		}
		
		public SpecialFunction Function
		{
			get
			{
				return _function;
			}
		}
	}
}
