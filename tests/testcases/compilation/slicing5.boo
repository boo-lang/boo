"""
Homer
Idle
Simpson
"""
import Boo.Lang.Compiler.Tests from Boo.Lang.Compiler.Tests

people = PersonCollection()
people.Add(Person(FirstName: "Homer", LastName: "Simpson"))
people.Add(Person(FirstName: "Eric", LastName: "Idle"))

print(people[0].FirstName)
print(people[1].LastName)
print(people["Homer"].LastName)
