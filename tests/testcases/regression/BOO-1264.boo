#ignore WIP
"""
add(IntNumber)
"""
interface INumber[of T]:
	def add(value as T)

class MyVector[of T(INumber[of T])]:
	x as T
	
	def constructor(x as T):
		self.x = x
		
	def add(b as MyVector[of T]):
		x.add(b.x)
		
class IntNumber(INumber[of IntNumber]):
	def add(value as IntNumber):
		print "add($value)"
		
MyVector[of IntNumber](IntNumber()).Add(IntNumber())
	
		

