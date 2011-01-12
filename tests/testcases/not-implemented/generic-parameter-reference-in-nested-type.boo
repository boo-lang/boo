"""
generic-parameter-reference-in-nested-type.boo(7,22): BCE0031: Language feature not implemented: referencing generic parameter of outer type.
generic-parameter-reference-in-nested-type.boo(8,46): BCE0031: Language feature not implemented: referencing generic parameter of outer type.
"""
class A[of T]:
	class B:
		f1 = List of T()
		f2 = System.Activator.CreateInstance(T)
		
