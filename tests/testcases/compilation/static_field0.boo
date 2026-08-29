"""
0
1
2
"""
class Person:

	public static InstanceCount as int
	
	def constructor():
		++InstanceCount

		
print(Person.InstanceCount)
p = Person()
print(Person.InstanceCount)
p = Person()
print(Person.InstanceCount)
