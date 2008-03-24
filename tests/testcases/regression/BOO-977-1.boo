"""
Foo`1[System.String]
"""

class Foo[of T]:
	pass

foo = Foo[of string]()

print foo.ToString()