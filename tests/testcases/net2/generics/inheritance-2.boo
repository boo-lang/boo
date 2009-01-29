"""
42
foo
84
bar
"""

class Base[of T]:
	def constructor(x as T):
		_x = x

	[getter(X)]
	_x as T

class Base[of T,U]:
	def constructor(x as T, y as U):
		_x = x
		_y = y

	[getter(X)]
	_x as T

	[getter(Y)]
	_y as U

class Test[of TX,TY] (Base[of TX,TY]):
	def constructor(x as TX, y as TY):
		super(x, y)

class Test2 (Base[of int,string]):
	def constructor(x as int, y as string):
		super(x, y)


t = Test[of int,string](42, "foo")
print t.X
print t.Y
t2 = Test2(84, "bar")
print t2.X
print t2.Y

