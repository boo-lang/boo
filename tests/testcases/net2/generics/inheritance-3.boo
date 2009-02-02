"""
Hello
"""
//BOO-1079

class B(A[of string]):
	def Foo():
		return Bar()

class A[of T]:
	def Bar():
		return "Hello"

b = B()
print b.Bar()

