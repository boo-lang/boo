"""
public class ClassWithoutConstructor(System.Object):

	public def run() as System.Void:
		Boo.Lang.Builtins.print('it worked!')

	public def constructor():
		super()

"""
class ClassWithoutConstructor(object):
	
	def run():
		print("it worked!")

