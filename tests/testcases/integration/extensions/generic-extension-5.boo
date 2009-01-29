"""
"""

[extension]
def IsTypeFullName[of T](s as string):
	return typeof(T).FullName == s

s = "System.String"
assert s.IsTypeFullName[of string]()
assert not s.IsTypeFullName[of int]()

