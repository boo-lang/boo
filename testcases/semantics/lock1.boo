"""
public final transient class Lock1Module(System.Object):

	private static def __Main__() as System.Void:
		__monitor2__ = System.Object()
		System.Threading.Monitor.Enter(__monitor2__)
		try:
			__monitor1__ = System.Object
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


