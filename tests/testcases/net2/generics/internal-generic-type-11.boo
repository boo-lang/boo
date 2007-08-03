"""
1
2
3
"""

public class GenericClass of T:
	public def constructor(argument as T):
		Field = argument

	public Field as T

	public Property as T:
		get: return Field
		set: Field = value

	public def Method(argument as T) as T:
		local as T = argument
		return local

a = GenericClass[of int](0)
a.Field = 1
print a.Field
print a.Property * 2
print a.Method(2) + 1

