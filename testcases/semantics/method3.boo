"""
public final transient class Method3Module(System.Object):

	public static def fatorial(n as System.Int32) as System.Int32:
		if (n > 1):
			return (n * Method3Module.fatorial((n - 1)))
		return 1

	private def constructor():
		super()

"""
def fatorial(n as int):	
	return n * fatorial(n-1) if n > 1
	return 1
