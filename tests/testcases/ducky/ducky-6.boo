"""
baz
Foo.Call(1, 1, 2)
Foo.Call(3, 5, 8)
Bar(spam, eggs)
Bar.Sprong
"""
class Foo(callable):
	def Call(args as (object)) as object:
		print "Foo.Call(${join(args, ', ')})"
		
class Bar:
	def constructor(first, second):
		print "Bar(${first}, ${second})"
		
	def Sprong():
		print "Bar.Sprong"
		
def baz():
	print "baz"

f as object
f = baz
f()

f = Foo()
f("1", "1", "2")
f(3, 5, 8)

f = Bar
b as Bar = f("spam", "eggs")
f = b.Sprong
f()


