"""
[Boo.Lang.ModuleAttribute]
public final transient class Method7Module(System.Object):

	public static def greeting(who as System.String) as System.Object:
		if System.String.op_Inequality(who, 'bamboo'):
			return 'wassup, g?'
		return Method7Module.g()

	public static def g() as System.Object:
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
