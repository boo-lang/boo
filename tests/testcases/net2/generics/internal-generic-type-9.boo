"""
1
2
3
4
"""

import System

public struct GenericStruct of T:
	public def constructor(argument as T):
		Field = argument

	public Field as T

	public Property as T:
		get: return Field
		set: Field = value

	public def Method(argument as T) as T:
		Event(argument) unless Event is null
		local as T = argument
		return local

	public event Event as Action of T

action as Action of int = { i as int | print i + 1 }

a = GenericStruct[of int](0)
a.Event += action
a.Field = 1
print a.Field
print a.Property * 2
print a.Method(2) * 2
a.Event -= action
