"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Lock1Module(object):

	private static def Main(argv as (string)) as void:
		__monitor2__ = object()
		System.Threading.Monitor.Enter(__monitor2__)
		try:
			__monitor1__ = object
			System.Threading.Monitor.Enter(__monitor1__)
			try:
				System.Console.WriteLine('spam')
			ensure:
				System.Threading.Monitor.Exit(__monitor1__)
		ensure:
			System.Threading.Monitor.Exit(__monitor2__)

	private def constructor():
		super()
"""
lock object(), object:
	System.Console.WriteLine('spam')


