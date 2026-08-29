"""
default B
arged A, passed called first
default A
arged B, passed B
"""
class A:
	def constructor(a as string):
		print "arged A, passed ${a}"
		
	def constructor():
		self("called first")
		print "default A"
				
class B(A):
	
	def constructor(b as string):
		super()
		print "arged B, passed ${b}"
		
	def constructor():
		print "default B"
		self("B")
		
B()
