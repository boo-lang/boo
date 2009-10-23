"""
BCE0175.boo(9,21): BCE0175: Nested type 'A.B.C.D' cannot extend enclosing type 'A.B'.
BCE0175.boo(8,17): BCE0175: Nested type 'A.B.C' cannot extend enclosing type 'A'.
BCE0175.boo(7,13): BCE0175: Nested type 'A.B' cannot extend enclosing type 'A'.
"""
class A:
	class B(A):
		class C(A):
			class D(B):
				pass
