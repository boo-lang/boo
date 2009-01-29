#ignore Boo's SRE of generic base type constraint of constraint's declaring type does not work on MS.NET (2.0 at least)
"""
"""

class Base[of T(Base[of T])]:
	pass

class Internal(Base[of Internal]):
	pass

assert Internal() != null

