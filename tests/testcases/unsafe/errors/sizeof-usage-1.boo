"""
sizeof-usage-1.boo(18,15): BCE0168: Cannot take the address of, get the size of, or declare a pointer to managed type `object'.
sizeof-usage-1.boo(19,15): BCE0168: Cannot take the address of, get the size of, or declare a pointer to managed type `object'.
sizeof-usage-1.boo(22,15): BCE0168: Cannot take the address of, get the size of, or declare a pointer to managed type `string'.
sizeof-usage-1.boo(23,15): BCE0168: Cannot take the address of, get the size of, or declare a pointer to managed type `string'.
sizeof-usage-1.boo(26,15): BCE0168: Cannot take the address of, get the size of, or declare a pointer to managed type `TestManaged'.
sizeof-usage-1.boo(27,15): BCE0168: Cannot take the address of, get the size of, or declare a pointer to managed type `TestManaged'.
sizeof-usage-1.boo(30,15): BCE0168: Cannot take the address of, get the size of, or declare a pointer to managed type `(string)'.
sizeof-usage-1.boo(33,15): BCE0168: Cannot take the address of, get the size of, or declare a pointer to managed type `(int)'.
"""

struct TestManaged:
	x as int
	y as int
	z as string

o as object
assert sizeof(o) == 42
assert sizeof(object) == 42

s = "foo"
assert sizeof(s) == 42
assert sizeof(string) == 42

tm = TestManaged()
assert sizeof(tm) == 42
assert sizeof(TestManaged) == 42

sv = ("x", "y", "z",)
assert sizeof(sv) == 42

iv = (1, 2, 3,)
assert sizeof(iv) == 42

