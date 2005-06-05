"""
[Boo.Lang.ModuleAttribute]
public final transient class Lock1Module(System.Object):

	private static def Main(argv as (System.String)) as System.Void:
		__monitor2__ = object()
		System.Threading.Monitor.Enter(__monitor2__)
		try:
			__monitor1__ = object
			System.Threading.Monitor.Enter(__monitor1__)
			try:
				pass
			ensure:
				System.Threading.Monitor.Exit(__monitor1__)
		ensure:
			System.Threading.Monitor.Exit(__monitor2__)

	private def constructor():
		super()
"""
lock object(), object:
	pass


