namespace Boo.Ast.Compilation.Binding
{
	public class AssemblyBinding : IBinding
	{
		System.Reflection.Assembly _assembly;
		
		public AssemblyBinding(System.Reflection.Assembly assembly)
		{
			if (null == assembly)
			{
				throw new System.ArgumentNullException("assembly");
			}
			_assembly = assembly;
		}
		
		public string Name
		{
			get
			{
				return _assembly.FullName;
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Assembly;
			}
		}
		
		public System.Reflection.Assembly Assembly
		{
			get
			{
				return _assembly;
			}
		}
	}
}
