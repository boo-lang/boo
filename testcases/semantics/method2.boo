"""
public final transient class Method2Module(System.Object):

	public static def fatorial(n as System.Int32) as System.Int32:
		if (n < 2):
			return 1
		return (n * Method2Module.fatorial((n - 1)))

	private def constructor():
		super()

"""
def fatorial(n as int):
	return 1 if n < 2
	return n * fatorial(n-1)
