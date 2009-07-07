class Foo:
	pass

b as bool

o as object = Foo()
b = not o
assert b is false
assert o

oo = Foo()
b = not oo
assert b is false

s as short = 42
b = not s
assert b is false

us as ushort = 42
b = not us
assert b is false

l = 1L
b = not l
assert b is false

f = 0.1f
b = not f
assert b is false

d = 0.1
b = not d
assert b is false
