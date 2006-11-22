import MixedBase from "mixedbase"

class Person:
	public Name as string

	public def constructor(name as string):
		Name = name

def GetPerson() as Person:
	return Person("Avish")

d = Derived of Person(Person("Avish"))
assert d.Field.Name == "Avish"
assert d.Property.Name == "Avish"
assert d.Method().Name == "Avish"

d.Event += GetPerson
d.Event -= GetPerson
