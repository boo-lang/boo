"""
42
"""

public interface GenericInterface[of T]:
	def Method(arg as T) as T

public class GenericType[of T] (GenericInterface of T):
	def Method(arg as T):
		return arg

assert typeof(GenericType of *).MakeGenericType(int) == typeof(GenericType of int)
assert typeof(GenericInterface of int) in typeof(GenericType of int).GetInterfaces()
print GenericType[of int]().Method(21) * 2