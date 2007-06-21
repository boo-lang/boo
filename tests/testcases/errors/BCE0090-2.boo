"""
BCE0090-2.boo(5,19): BCE0090: Derived method 'A.ToString' can not reduce the accessibility of 'object.ToString' from 'public' to 'protected'.
"""
class A:
	protected def ToString():
		return "A"

