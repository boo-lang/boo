"""
public final transient class Lock0Module(System.Object):

	private static def __Main__() as System.Void:
		o1 = System.Object()
		__monitor1__ = o1
		System.Threading.Monitor.Enter(__monitor1__)
		try:
			pass
		ensure:
			System.Threading.Monitor.Leave(__monitor1__)
		o2 = System.Object()
		__monitor2__ = o1
		System.Threading.Monitor.Enter(__monitor2__)
		try:
			__monitor3__ = o2
			System.Threading.Monitor.Enter(__monitor3__)
			try:
				pass
			ensure:
				System.Threading.Monitor.Leave(__monitor3__)
		ensure:
			System.Threading.Monitor.Leave(__monitor2__)

	private def constructor():
		super()
"""
o1 = object()
lock o1:
	pass
	
o2 = object()
lock o1, o2:
	pass


