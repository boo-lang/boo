"""
-1
-2
"""

import System.Collections.Generic

public class Person:
	public Name as string

	public def constructor(name as string):
		Name = name

public class PersonComparer(Comparer of Person):
	public override def Compare(x as Person, y as Person):
		return string.Compare(x.Name, y.Name)

public class IntComparer(Comparer of int):
	public override def Compare(x as int, y as int):
		return x - y

p1 = Person("Avish")
p2 = Person("Bet's On!")
print PersonComparer().Compare(p1, p2)
print IntComparer().Compare(1, 3)
