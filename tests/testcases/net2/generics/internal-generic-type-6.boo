"""
42
"""

public class GenericType[of T]: 
	public Field as T

a = GenericType[of int]()
a.Field = 21
print a.Field * 2
