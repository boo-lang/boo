namespace Boo.Lang.Compiler.Pipelines
{
	public class Quack : Run
	{
		public Quack()
		{
			MakeItQuack(this);
		}
		
		public static CompilerPipeline MakeItQuack(CompilerPipeline pipeline)
		{
			int index = pipeline.Find(typeof(Boo.Lang.Compiler.Steps.ProcessMethodBodies));
			pipeline[index] = new Boo.Lang.Compiler.Steps.HuntDucks();
			return pipeline;
		}
	}
}
