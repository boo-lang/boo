"""
public final transient class Method6Module(System.Object):

	public static def greeting(who as System.String) as System.String:
		if System.String.op_Inequality(who, 'bamboo'):
			return 'wassup, g?'
		return Method6Module.g()

	public static def g() as System.String:
		return Method6Module.greeting('g')

	private def constructor():
		super()


"""
def greeting(who as string) as string:
	return "wassup, g?" if who != "bamboo"
	return g()
	
def g():
	return greeting("g")
