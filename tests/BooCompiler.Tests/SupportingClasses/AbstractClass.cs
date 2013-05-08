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

	public abstract class A1
	{
		public abstract int P1 { get; }
	}

	public abstract class A2:A1
	{
	}
}