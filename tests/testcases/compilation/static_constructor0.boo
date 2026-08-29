"""
static constructor called
0
1
2
"""
class Person:

	public static InstanceCount as int
	
	def constructor():
		++InstanceCount
		
	static def constructor():
		print("static constructor called")

		
print(Person.InstanceCount)
p = Person()
print(Person.InstanceCount)
p = Person()
print(Person.InstanceCount)
