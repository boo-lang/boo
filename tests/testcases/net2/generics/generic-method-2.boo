"""
System.String
Hello
"""

def Method[of T](parameter as T):
	print parameter.GetType()
	print parameter

Method[of string]("Hello")
