namespace BooCompiler.Tests.SupportingClasses
{
	public class ReturnDucks
	{
		public class DuckBase {}
		
		public class DuckFoo : DuckBase
		{
			public string Foo() { return "foo"; }
		}
		
		public class DuckBar : DuckBase
		{
			public string Bar() { return "bar"; }
		}
		
		[Boo.Lang.DuckTyped]
		public DuckBase GetDuck(bool foo)
		{
			if (foo) return new DuckFoo();
			return new DuckBar();
		}
	}
}