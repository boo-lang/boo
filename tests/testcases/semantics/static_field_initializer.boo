"""
public class Person(System.Object):

	public static InstanceCount as System.Int32

	public def constructor():
		super()
		Person.InstanceCount = (Person.InstanceCount + 1)

	static def ___static_initializer() as System.Void:
		Person.InstanceCount = 0

	public static def constructor():
		Person.___static_initializer()
"""
class Person:

	public static InstanceCount = 0

	def constructor():
		++InstanceCount

