"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Method2Module(object):

	public static def fatorial(n as int) as int:
		if n < 2:
			return 1
		return (n * Method2Module.fatorial((n - 1)))

	private def constructor():
		super()

"""
def fatorial(n as int) as int:
	return 1 if n < 2
	return n * fatorial(n-1)
