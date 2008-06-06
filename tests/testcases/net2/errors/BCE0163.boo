"""
BCE0163.boo(11,25): BCE0163: Type constraint 'SomeOtherClass' on generic parameter 'T' conflicts with type constraint 'SomeClass'. At most one non-interface type constraint can be specified for a generic parameter.
"""

class SomeClass:
	pass
	
class SomeOtherClass:
	pass
	
class C[of T(SomeClass, SomeOtherClass)]:
	pass
