namespace BooCompiler.Tests.SupportingClasses
{
	public class PointersBase
	{
		public void Foo(ref int bar)
		{
			System.Console.WriteLine("PointersBase.Foo(int&)");
		}

		public unsafe void Foo(int* bar)
		{
			System.Console.WriteLine("Pointers.Foo(int*)");
		}
	}

	public class Pointers : PointersBase
	{
		public new void Foo(ref int bar)
		{
			System.Console.WriteLine("Pointers.Foo(int&)");
		}
	}
}