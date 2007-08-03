public interface GenericInterface of T:
	pass

assert typeof(GenericInterface of *).MakeGenericType(int) == typeof(GenericInterface of int)
