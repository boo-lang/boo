"""
A => Avish
B => Bamboo
C => Cedric
"""
import System.Collections.Generic

public class Person:
	public Name as string

	public def constructor(name as string):
		Name = name

persons = Dictionary[of Person, Person]()

persons.Add(Person("A"), Person("Avish"))
persons.Add(Person("B"), Person("Bamboo"))
persons.Add(Person("C"), Person("Cedric"))

for kvp in persons:
	print "${kvp.Key.Name} => ${kvp.Value.Name}"

