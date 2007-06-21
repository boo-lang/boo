interface IFoo:
	Bar:
		get
		
class Foo(IFoo):
	Bar as object:
		get:
			return "Bar"
			

f as IFoo = Foo()
assert "Bar" == f.Bar
