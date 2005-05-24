namespace BooWebService.Server

import System.Web.Services

class Person:
"""
The application is built around plain objects.
"""
	[property(Name)]
	_name as string
	
	def constructor(name as string):
		_name = name
		

class PersonData:
"""
The world talks to the application through data objects
which assemble the data and the db4o id together.
"""	
	public Id as long
	
	public Name as string
	
	
[WebService]
class PersonWebServiceImpl:

	[WebMethod]
	def Save(data as PersonData):
	"""
	Updates an existing Person or creates a new one.
	"""
		container = PersonApplication.OpenSession()
		try:
			person as Person = container.ext().getByID(data.Id)
			if person is null:
				person = Person(data.Name)
			else:
				person.Name = data.Name
			container.set(person)
		ensure:
			container.close()
			
	[WebMethod]
	def QueryAll() as (PersonData):
	"""
	Returns all existing Person instances.
	"""
		container = PersonApplication.OpenSession()
		try:
			data = []
			os = container.get(Person)
			while os.hasNext():
				person as Person = os.next()
				data.Add(
					PersonData(
						Id: container.ext().getID(person),
						Name: person.Name))
			return data.ToArray(PersonData)
		ensure:
			container.close()
