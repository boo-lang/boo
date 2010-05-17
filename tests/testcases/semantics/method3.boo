"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Method3Module(object):

	public static def fatorial(n as int) as int:
		if n > 1:
			return (n * Method3Module.fatorial((n - 1)))
		return 1

	private def constructor():
		super()

"""
def fatorial(n as int) as int:	
	return n * fatorial(n-1) if n > 1
	return 1
