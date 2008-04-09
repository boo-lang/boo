"""
Param
"""
class Param:
	pass

c = (Param(),)
d = System.Array.AsReadOnly[of Param](c)

print d[0].GetType()