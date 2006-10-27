"""
-1
"""

import System
import System.Collections.Generic

public class Person:
	public Name as string

	public def constructor(name as string):
		Name = name

public class PersonComparer(IComparer of Person):
	public def Compare(x as Person, y as Person):
		return string.Compare(x.Name, y.Name)

p1 = Person("Avish")
p2 = Person("Bet's On!")
c = PersonComparer() as IComparer of Person
print c.Compare(p1, p2)

