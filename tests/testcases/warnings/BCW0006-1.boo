"""
BCW0006-1.boo(13,12): BCW0006: WARNING: Assignment to temporary.
"""
struct Foo:
	value as int
	
class Bar:
	_f = Foo()
	def GetFoo():
		return _f
	
b = Bar()
b.GetFoo().value = 3
