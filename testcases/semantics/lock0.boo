"""
public final transient class Lock0Module(System.Object):

	private static def __Main__() as System.Void:
		o1 = System.Object()
		__monitor1__ = o1
		System.Threading.Monitor.Enter(__monitor1__)
		try:
			pass
		ensure:
			System.Threading.Monitor.Exit(__monitor1__)

	private def constructor():
		super()
"""
o1 = object()
lock o1:
	pass
