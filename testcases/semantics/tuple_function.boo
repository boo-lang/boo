"""
public final transient class Tuple_functionModule(System.Object):

	private static def __Main__() as System.Void:
		for a as System.Int32 in tuple(int, range(10)):
			print(a)
		for a as System.Object in tuple(range(10)):
			print(a)

	private def constructor():
		super()
"""
for a in tuple(int, range(10)):
	print(a)
	
for a in tuple(range(10)):
	print(a)
