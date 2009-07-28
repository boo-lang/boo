"""
23
23
42
42
"""

class Param[of T]:
	[getter(Value)]
	value as T

	def constructor(value as T):
		.value = value


def Foo[of T](*params as (Param[of T])):
	for p in params:
		print p.Value if p

def Foo[of T](x as int, *params as (Param[of T])):
	for p in params:
		print p.Value if p



a = Param[of int](23)
b = Param[of int](42)
Foo(a)
Foo(a, b, null)
Foo(0, b)

