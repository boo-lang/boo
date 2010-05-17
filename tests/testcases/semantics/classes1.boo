"""
public class ClassWithoutConstructor(object):

	public def run() as void:
		Boo.Lang.Builtins.print('it worked!')

	public def constructor():
		super()

"""
class ClassWithoutConstructor(object):
	
	def run():
		print("it worked!")

