"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Null0Module(object):

	public static def foo(n as bool) as string:
		if n:
			return null
		return 'null'

	public static def bar(n as bool) as string:
		if n:
			return 'null'
		return null

	public static def baz(n as bool) as object:
		if n:
			return null
		return 14

	private def constructor():
		super()

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
