"""
public final transient class Null0Module(System.Object):

	public static def foo(n as System.Boolean) as System.String:
		if n:
			return null
		return 'null'

	private def constructor():
		pass

"""
def foo(n as bool):
	return null if n
	return "null"
