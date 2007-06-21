"""
4 True
5 True
"""

interface IDoit:
	def doit(ref x as int)

class A(IDoit):
	virtual def doit(ref x as int):
		++x

class B(A):
	override def doit(ref x as int):
		++x
		super(x)
a = A()
x = 3
a.doit(x)
print x, x==4

b = B()
x = 3
b.doit(x)
print x, x==5

