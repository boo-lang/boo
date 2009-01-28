"""
4
4
42
>>>
1 a
3  b
1 a2
<<<
foo2.bar
"""
###external nested macro extensions testcase
import BooSupportingClasses.NestedMacros #foo foo2 foo2.root are in BooSupportingClasses.dll

macro foo.bar:
	yield [| print $(foo.Arguments.Count) |]

macro foo2.bar:
	yield [| print "foo2.bar" |]

macro foo2.root.a:
	yield [| print "${$(foo2.Arguments.Count)} a" |]
	yield

macro foo2.root.a2:
	yield [| print "${$(foo2.Arguments.Count)} a2" |]

macro foo2.root.a.b:
	yield [| print "${$(a.Arguments.Count)}  b" |]


def bar(x as Foo2Macro, y as int) as FooMacro:
	print y
	return null

def bar(x,y):
	return true


foo 1, 2, 3, 4:
	bar
	bar
	bar(Foo2Macro(), 42) #not a macro, resolved to the method above

foo2 foo:
	root:
		a 1, 2, 3:
			b
		a2
	bar

assert bar(0,1)

