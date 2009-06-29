"""
BCE0168-1.boo(19,21): BCE0168: Cannot take the address of, get the size of, or declare a pointer to managed type `string'.
BCE0168-1.boo(23,20): BCE0168: Cannot take the address of, get the size of, or declare a pointer to managed type `object'.
BCE0168-1.boo(28,6): BCE0168: Cannot take the address of, get the size of, or declare a pointer to managed type `object'.
BCE0168-1.boo(31,20): BCE0168: Cannot take the address of, get the size of, or declare a pointer to managed type `EmptyFoo'.
BCE0168-1.boo(35,21): BCE0168: Cannot take the address of, get the size of, or declare a pointer to managed type `Foo'.
"""


struct EmptyFoo:
	pass

struct Foo:
	x as int
	y as object


s = "foo"
unsafe sp as byte = s:
	*sp = 1

objects = array[of object](1)
unsafe op as int = objects:
	_ = 0

bytes = array[of byte](1)
unsafe bp as object = bytes:
	*bp = null

f = EmptyFoo()
unsafe fp as int = f:
	_ = 0

vf = array[of Foo](1)
unsafe vfp as Foo = vf:
	_ = 0

