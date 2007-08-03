"""
42
"""

public class GenericType of T: 
	def Method(argument as T) as T:
		return argument

print GenericType[of int]().Method(21) * 2
