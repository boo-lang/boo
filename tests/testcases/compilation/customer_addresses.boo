"""
foo street, 50
"""
class Address:
	
	[getter(Street)]
	_street as string
	
	[getter(Number)]
	_number as int
	
	def constructor(street as string, number as int):
		_street = street
		_number = number
		
	override def ToString():
		return "${_street}, ${_number}"
		
class Customer:

	[getter(Addresses)]
	_addresses = []	
	
	[getter(Name)]
	_name as string
	
	def constructor([required] name as string):
		_name = name
			
c = Customer("Homer Simpson")
c.Addresses.Add(Address("foo street", 50))

print(c.Addresses[0])
