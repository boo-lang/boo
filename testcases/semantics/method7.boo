"""
public final transient class Method7Module(System.Object):

	public static def greeting(who as System.String) as System.Object:
		if (who != 'bamboo'):
			return 'wassup, g?'
		return g()

	public static def g() as System.Object:
		number = System.Random().Next(10)
		if (1 == number):
			return greeting('g')
		return number

	private def constructor():
		pass


"""
def greeting(who as string):
	return "wassup, g?" if who != "bamboo"
	return g()
	
def g():
	number = System.Random().Next(10)
	return greeting("g") if 1 == number
	return number
