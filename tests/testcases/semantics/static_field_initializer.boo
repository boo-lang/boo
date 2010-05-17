"""
public class Person(object):

	public static InstanceCount as int

	public def constructor():
		super()
		Person.InstanceCount = (Person.InstanceCount + 1)

	private static def constructor():
		Person.InstanceCount = 1
"""
class Person:

	public static InstanceCount = 1

	def constructor():
		++InstanceCount

