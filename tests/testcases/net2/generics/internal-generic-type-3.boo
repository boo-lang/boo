public struct GenericStruct of T:
	pass

assert typeof(GenericStruct of *).MakeGenericType(int) == typeof(GenericStruct of int)