"""
BCE0159.boo(5,13): BCE0159: Generic parameter 'T' cannot have both a reference type constraint and a value type constraint.
"""

class C1[of T(struct, class)]:
	pass
