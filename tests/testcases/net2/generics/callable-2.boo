"""
Avish
"""

import System
import System.Collections.Generic

public class Person:
	public Name as string
	public def constructor(name as string):
		Name = name

f as Action of Person
f = {p as Person | print p.Name}

l = List of Person()
l.Add(Person("Avish"))
l.ForEach(f)
