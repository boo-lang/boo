"""
public final transient class Null0Module(System.Object):

	public static def foo(n as System.Boolean) as System.String:
		if n:
			return null
		return 'null'

	public static def bar(n as System.Boolean) as System.String:
		if n:
			return 'null'
		return null

	public static def baz(n as System.Boolean) as System.Object:
		if n:
			return null
		return 14

	private def constructor():
		pass

"""
def foo(n as bool):
	return null if n
	return "null"
	
def bar(n as bool):
	return "null" if n
	return null
	
def baz(n as bool):
	return null if n
	return 14
