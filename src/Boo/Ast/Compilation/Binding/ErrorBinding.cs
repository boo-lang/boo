namespace Boo.Ast.Compilation.Binding
{
	public class ErrorBinding : ITypeBinding, INamespace
	{
		public static ErrorBinding Default = new ErrorBinding();
		
		private ErrorBinding()
		{			
		}
		
		public string Name
		{
			get
			{
				return "Error";
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Error;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return this;
			}
		}
		
		public System.Type Type
		{
			get
			{
				return Types.Void;
			}
		}
		
		public IConstructorBinding[] GetConstructors()
		{
			return new IConstructorBinding[0];
		}
		
		public IBinding Resolve(string name)
		{
			return null;
		}
	}
}
