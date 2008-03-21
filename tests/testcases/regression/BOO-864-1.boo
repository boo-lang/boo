interface IFoo:
	pass

interface IBar(IFoo):
	pass

class BaseFoo(IFoo):
	pass

class MyFoo(BaseFoo, IBar):
	pass
	
assert MyFoo() isa IFoo
assert MyFoo() isa IBar
