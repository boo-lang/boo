"""
Guido
Matz
"""
import BooCompiler.Tests from BooCompiler.Tests

people = PersonCollection()
people.Add(Person(FirstName: "Homer", LastName: "Simpson"))
people.Add(Person(FirstName: "Eric", LastName: "Idle"))

people[0] = Person(FirstName: "Guido")
people["Eric"] = Person(FirstName: "Matz")

print(people[0].FirstName)
print(people[1].FirstName)
