"""
B: foo!
B: bar!
"""
class A:
	virtual def Foo(arg):
		yield "foo" + arg 
		yield "bar" + arg
		
class B(A):
	override def Foo(arg):
		for i in super(arg):
			yield "B: " + i
			
for i in B().Foo("!"):
	print i
