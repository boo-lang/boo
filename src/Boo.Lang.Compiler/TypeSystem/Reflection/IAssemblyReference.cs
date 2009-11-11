namespace Boo.Lang.Compiler.TypeSystem.Reflection
{
	public interface IAssemblyReference : ICompileUnit
	{
		System.Reflection.Assembly Assembly { get;  }
	}
}
