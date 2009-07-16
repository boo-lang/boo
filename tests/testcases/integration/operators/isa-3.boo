"""
woot! foo is a string!
0 is not a string :-(
woot! bar is a string!
woot! baz is a string!
"""

def Foo[of T](x as T):
	if x isa string:
		print "woot! ${x} is a string!"
	else:
		print "${x} is not a string :-("


def FooClass[of T(class)](x as T):
	if x isa string:
		print "woot! ${x} is a string!"


Foo[of string]("foo")
Foo[of int](0)

FooClass[of string]("bar")
FooClass[of object]("baz")
FooClass[of object](object())
FooClass[of object](null)

