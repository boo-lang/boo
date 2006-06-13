"""
Homer
Idle
Simpson
"""
import BooCompiler.Tests from BooCompiler.Tests

people = PersonCollection()
people.Add(Person(FirstName: "Homer", LastName: "Simpson"))
people.Add(Person(FirstName: "Eric", LastName: "Idle"))

print(people[0].FirstName)
print(people[1].LastName)
print(people["Homer"].LastName)
