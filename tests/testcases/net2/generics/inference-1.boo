"""
Int32
"""

def Method[of T](arg as T):
	return typeof(T).Name
	
print Method(42)