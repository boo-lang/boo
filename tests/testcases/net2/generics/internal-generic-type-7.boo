"""
42
"""

public class GenericType[of T]: 
	public def Method(argument as T):
		local as T = argument
		return local

print GenericType[of int]().Method(21) * 2

