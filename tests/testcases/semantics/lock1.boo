"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Lock1Module(object):

	private static def Main(argv as (string)) as void:
		\$lock\$monitor\$2 = object()
		System.Threading.Monitor.Enter(\$lock\$monitor\$2)
		try:
			\$lock\$monitor\$1 = object
			System.Threading.Monitor.Enter(\$lock\$monitor\$1)
			try:
				System.Console.WriteLine('spam')
			ensure:
				System.Threading.Monitor.Exit(\$lock\$monitor\$1)
		ensure:
			System.Threading.Monitor.Exit(\$lock\$monitor\$2)

	private def constructor():
		super()
"""
lock object(), object:
	System.Console.WriteLine('spam')


