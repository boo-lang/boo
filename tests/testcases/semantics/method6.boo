"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Method6Module(object):

	public static def greeting(who as string) as string:
		if string.op_Inequality(who, 'bamboo'):
			return 'wassup, g?'
		return Method6Module.g()

	public static def g() as string:
		return Method6Module.greeting('g')

	private def constructor():
		super()


"""
def greeting(who as string) as string:
	return "wassup, g?" if who != "bamboo"
	return g()
	
def g():
	return greeting("g")
