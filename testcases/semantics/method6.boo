"""
public final transient class Method6Module(System.Object):

	public static def greeting(who as string) as System.String:
		if (who != 'bamboo'):
			return 'wassup, g?'
		return g()

	public static def g() as System.String:
		return greeting('g')

	private def constructor():
		pass


"""
def greeting(who as string):
	return "wassup, g?" if who != "bamboo"
	return g()
	
def g():
	return greeting("g")
