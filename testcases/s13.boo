class Person:

	_id as string

	_fname as string

	def constructor(id, fname):
		_id = id
		_fname = fname

	def getID() as string:
		return _id

	def getFirstName() as string:
		return _fname

	public static def Dump(p as Person):
		print("ID: ${p._id}\nFirst Name: ${p._fname}")


p = Person("1111", "Bamboo")
Person.Dump(p)