#ignore generics with nested types not supported yet
"""
System.String
"""
class A[of T]:
	class B:
		def Go():
			print typeof(T).FullName
			
A[of string].B().Go()
