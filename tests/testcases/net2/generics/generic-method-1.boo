"""
System.Int32
42
"""

def Method[of T](parameter as T):
	print parameter.GetType()
	print parameter

Method[of int](42)
