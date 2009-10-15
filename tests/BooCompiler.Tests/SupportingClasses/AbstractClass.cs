namespace BooCompiler.Tests.SupportingClasses
{
	public abstract class AbstractClass
	{
	}

	public abstract class AnotherAbstractClass
	{
		protected abstract string Foo();

		public virtual string Bar()
		{
			return "Bar";
		}
	}
}