"""
Homer
Idle
Simpson
"""
import Boo.Tests.Lang.Compiler from Boo.Tests

people = PersonCollection()
people.Add(Person(FirstName: "Homer", LastName: "Simpson"))
people.Add(Person(FirstName: "Eric", LastName: "Idle"))

print(people[0].FirstName)
print(people[1].LastName)
print(people["Homer"].LastName)
