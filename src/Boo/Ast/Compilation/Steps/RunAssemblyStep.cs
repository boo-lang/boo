namespace Boo.Ast.Compilation.Steps
{
	public class RunAssemblyStep : AbstractCompilerStep
	{
		public override void Run()
		{
			if (Errors.Count > 0 || CompilerOutputType.Library == CompilerParameters.OutputType)
			{
				return;
			}
			
			System.Reflection.MethodInfo method = EmitAssemblyStep.GetEntryPoint(CompileUnit);
			method.Invoke(null, null);
		}
	}
}
