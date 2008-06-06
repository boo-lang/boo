public class GenericType[of T]:
	pass

assert typeof(GenericType of *).MakeGenericType(int) == typeof(GenericType of int)
