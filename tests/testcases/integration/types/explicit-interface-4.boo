"""
A
B
"""
interface I:
	def Foo()

class A(I):
	def I.Foo():
		print "A"

class B(A):
	def I.Foo():
		print "B"
		
def foo(i as I):
	i.Foo()
	
foo A()
foo B()


