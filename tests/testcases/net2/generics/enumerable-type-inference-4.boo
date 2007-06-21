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

strings = array("${kvp.Key.Name} => ${kvp.Value.Name}" for kvp in persons)
System.Array.Sort[of string](strings)
for s in strings: print s

