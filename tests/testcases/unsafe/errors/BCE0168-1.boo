"""
BCE0168-1.boo(14,21): BCE0168: Cannot take the address of, get the size of, or declare a pointer to managed type `string'.
BCE0168-1.boo(18,20): BCE0168: Cannot take the address of, get the size of, or declare a pointer to managed type `object'.
BCE0168-1.boo(23,6): BCE0168: Cannot take the address of, get the size of, or declare a pointer to managed type `object'.
BCE0168-1.boo(26,20): BCE0168: Cannot take the address of, get the size of, or declare a pointer to managed type `Foo'.
"""



struct Foo:
	pass

s = "foo"
unsafe sp as byte = s:
	*sp = 1

objects = array[of object](1)
unsafe op as int = objects:
	_ = 0

bytes = array[of byte](1)
unsafe bp as object = bytes:
	*bp = null

f = Foo()
unsafe fp as int = f:
	_ = 0

