"""
Alice
Bob
Charlie
"""

import System.Collections.Generic

class Person:
	[property(Name)]
	_name as string

def YieldPersons():
	yield Person(Name: "Alice")
	yield Person(Name: "Bob")
	yield
	yield Person(Name: "Charlie")

for p in YieldPersons():
	print p.Name if p
