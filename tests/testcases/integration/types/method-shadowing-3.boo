"""
A.Foo
A.Foo
B.Foo
"""
class A:
	virtual def Foo():
		print "A.Foo" 
		
class B(A):
	new virtual def Foo():
		print "B.Foo"
		
def FooOn(a as A):
	a.Foo()
	
FooOn A()
FooOn B()
B().Foo()
		
	
