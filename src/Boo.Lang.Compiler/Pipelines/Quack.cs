namespace Boo.Lang.Compiler.Pipelines
{
	public class Quack : Run
	{
		public Quack()
		{
			int index = Find(typeof(Boo.Lang.Compiler.Steps.ProcessMethodBodies));
			this[index] = new Boo.Lang.Compiler.Steps.HuntDucks();
		}
	}
}
