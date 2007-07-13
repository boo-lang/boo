"""
public class Person(object):

	public static InstanceCount as int

	public def constructor():
		super()
		Person.InstanceCount = (Person.InstanceCount + 1)

	public static def constructor():
		Person.InstanceCount = 0
"""
class Person:

	public static InstanceCount = 0

	def constructor():
		++InstanceCount

