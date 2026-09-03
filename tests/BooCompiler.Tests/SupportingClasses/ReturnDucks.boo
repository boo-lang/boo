namespace BooCompiler.Tests.SupportingClasses

class ReturnDucks:
	class DuckBase:
		pass

	class DuckFoo(DuckBase):
		public def Foo() as string:
			return "foo"

	class DuckBar(DuckBase):
		public def Bar() as string:
			return "bar"

	[Boo.Lang.DuckTyped]
	public def GetDuck(foo as bool) as DuckBase:
		return DuckFoo() if foo
		return DuckBar()
