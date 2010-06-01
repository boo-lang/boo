"""
B: foo
B: bar
"""
class A:
	virtual def Foo():
		yield "foo" 
		yield "bar"
		
class B(A):
	override def Foo():
		for i in super.Foo():
			yield "B: " + i
			
for i in B().Foo():
	print i
