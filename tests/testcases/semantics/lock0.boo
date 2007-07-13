"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Lock0Module(object):

	private static def Main(argv as (string)) as void:
		o1 = object()
		__monitor1__ = o1
		System.Threading.Monitor.Enter(__monitor1__)
		try:
			System.Console.WriteLine('spam')
		ensure:
			System.Threading.Monitor.Exit(__monitor1__)

	private def constructor():
		super()
"""
o1 = object()
lock o1:
	System.Console.WriteLine('spam')
