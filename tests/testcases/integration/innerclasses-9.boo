class A:
	class B:
		pass
		
	class C(B):
		pass
		
assert typeof(A.C).BaseType is A.B
