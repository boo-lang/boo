"""
Avish
Avish
"""

# import System.ComponentModel
# import System.Collections.ObjectModel

class Person:
	public Name as string

	public def constructor(name as string):
		Name = name

l1 = System.Collections.ObjectModel.Collection of Person()
l2 = System.ComponentModel.BindingList of Person()

l1.Add(Person("Avish"))
l2.Add(Person("Avish"))

print l1[0].Name
print l2[0].Name




