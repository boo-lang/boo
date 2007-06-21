"""
BCE0117-1.boo(8,1): BCE0117: Field 'Person.FirstName' is read only.
"""
class Person:
	public final FirstName = "Homer"
	
p = Person()
p.FirstName = "Apu"
