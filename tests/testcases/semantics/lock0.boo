"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Lock0Module(object):

	private static def Main(argv as (string)) as void:
		o1 = object()
		\$lock\$monitor\$1 = o1
		System.Threading.Monitor.Enter(\$lock\$monitor\$1)
		try:
			System.Console.WriteLine('spam')
		ensure:
			System.Threading.Monitor.Exit(\$lock\$monitor\$1)

	private def constructor():
		super()
"""
o1 = object()
lock o1:
	System.Console.WriteLine('spam')
