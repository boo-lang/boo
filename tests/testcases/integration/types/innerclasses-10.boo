class A:
	interface B:
		pass
		
	class C(B):
		pass

class D(A.B):
	pass
		
assert A.B in typeof(A.C).GetInterfaces()
assert A.B in typeof(D).GetInterfaces()
