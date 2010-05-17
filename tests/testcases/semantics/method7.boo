"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Method7Module(object):

	public static def greeting(who as string) as object:
		if string.op_Inequality(who, 'bamboo'):
			return 'wassup, g?'
		return Method7Module.g()

	public static def g() as object:
		number = System.Random().Next(10)
		if 1 == number:
			return Method7Module.greeting('g')
		return number

	private def constructor():
		super()


"""
def greeting(who as string) as object:
	return "wassup, g?" if who != "bamboo"
	return g()
	
def g():
	number = System.Random().Next(10)
	return greeting("g") if 1 == number
	return number
