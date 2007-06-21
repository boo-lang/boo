class Test:
	public x as double
	
	def Equals(y as object) as bool:
		return cast(double, y) == x

o1 as object = 1.0
o2 as object = 1
d1 as duck = 1.0
d2 as duck = 1
t1 = Test(x: 1)

assert o1 == 1
assert 1 == o1
assert o1 == o2
assert o2 == o1
assert d1 == 1
assert 1 == d1
assert d1 == d2
assert d2 == d1
assert o1 = d2
assert d2 = o1
assert t1 == o2
assert o2 == t1 // fails
