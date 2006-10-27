"""
Avish
Avish
Avish
"""

import System.Collections.Generic

class Person:
	public Name as string

	public def constructor(name as string):
		Name = name

def MatchName(name as string):
	return {p as Person | p.Name == name}

l = List of Person()
l.Add(Person("Avish"))

print l[0].Name
print l.Find(MatchName("Avish")).Name
print l.Find({p as Person | p.Name == "Avish"}).Name
