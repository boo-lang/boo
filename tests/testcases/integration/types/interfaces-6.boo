
interface IFoo:
	pass
	
interface IBar(IFoo):
	pass
	
class Foo(IBar):
	pass

foo = Foo()
assert foo isa IBar
assert foo isa IFoo
