"""System.Int32 System.String"""

class GenericType[of T]:
	static def GenericMethod[of U](parameter as U):
		print typeof(T), typeof(U)

arg as string
GenericType[of int].GenericMethod(arg)
