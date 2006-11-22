namespace BooCompiler.Tests
{
	using Boo.Lang.Compiler;	

	public class AbstractCompilerErrorsTestFixture : AbstractCompilerTestCase
	{			
		public class PrintErrors : Boo.Lang.Compiler.Pipelines.Compile
		{
			override public void Run(CompilerContext context)
			{
				base.Run(context);
				RunStep(context, new Boo.Lang.Compiler.Steps.PrintErrors());
			}
		}
		
		protected override CompilerPipeline SetUpCompilerPipeline()
		{
			return new PrintErrors();
		}
		
		protected override bool IgnoreErrors
		{
			get
			{
				return true;
			}
		}
	}
}
