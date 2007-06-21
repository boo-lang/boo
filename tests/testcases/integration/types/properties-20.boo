interface IFoo:
	Bar:
		get
		
class Foo(IFoo):
	[property(Bar)]
	_bar

f as IFoo = Foo(Bar: "value")
assert "value" == f.Bar
