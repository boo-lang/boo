"""
public class ClassWithoutConstructor(System.Object):

	public def run() as System.Void:
		print('it worked!')

	public def constructor():
		pass

"""
class ClassWithoutConstructor(object):
	
	def run():
		print("it worked!")

