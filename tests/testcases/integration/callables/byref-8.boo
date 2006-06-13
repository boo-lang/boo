"""
10 20 True True
33 33 True True
2
"""


class A:
	public One as int
	public Two as int
	
	def doit():
		doit2(One)
		doit2(Two)
	def doit2(ref x as int):
		x = 33
		
	def constructor():
		super()
		
	def constructor(ref x as int):
		x = 2

def testclass(ref c as A):
	c = A(One:10, Two:20)

c = A(One:1, Two:2)
testclass(c)
print c.One, c.Two, c.One==10, c.Two==20

c.doit()
print c.One, c.Two, c.One==33, c.Two==33

x = 1
c2 = A(x)
c2 = c2 //just to avoid warning
print x

