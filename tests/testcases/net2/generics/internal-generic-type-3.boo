public struct GenericStruct of T:
	Field as T

assert typeof(GenericStruct of *).MakeGenericType(int) == typeof(GenericStruct of int)
