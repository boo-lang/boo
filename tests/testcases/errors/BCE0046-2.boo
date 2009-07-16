"""
BCE0046-2.boo(7,8): BCE0046: 'isa' can't be used with a value type ('T')
BCE0046-2.boo(11,8): BCE0046: 'isa' can't be used with a value type ('T')
"""

def Foo[of T(struct)](x as T):
	if x isa string:
		pass

def FooInt[of T(int)](x as T):
	if x isa string:
		pass


Foo[of int](0)
FooInt[of int](0)

