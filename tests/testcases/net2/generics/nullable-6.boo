class Foo:
	_has as bool

	def constructor(x as int?):
		_has = x.HasValue

def IsFortyTwoOrNull(x as int?) as bool:
	return x == 42 or not x.HasValue

i = 23
assert not IsFortyTwoOrNull(i)
assert IsFortyTwoOrNull(42)
assert IsFortyTwoOrNull(null)

Foo(10)
Foo(null)

