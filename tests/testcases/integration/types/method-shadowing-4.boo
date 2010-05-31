"""
A.Foo
A.Foo
A.Foo
B.Foo
C.Foo
"""
class A:
	virtual def Foo():
		print "A.Foo" 
		
class B(A):
	new virtual def Foo():
		print "B.Foo"
		
class C(B):
	override def Foo():
		print "C.Foo"
		
def AFooOn(a as A):
	a.Foo()

def BFooOn(b as B):
	b.Foo()

AFooOn A()
AFooOn B()
AFooOn C()
BFooOn B()
BFooOn C()
