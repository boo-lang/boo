"""
class Person:

	_id as string

	_fname as string

	def constructor(id as string, fname as string):
		_id = id
		_fname = fname

	def getID() as string:
		return _id

	def getFirstName() as string:
		return _fname

	def dump():
		pass

p = Person('1111', 'Bamboo')
p.dump()
"""
class Person:

	_id as string

	_fname as string

	def constructor(id as string, fname as string):
		_id = id
		_fname = fname

	def getID() as string:
		return _id

	def getFirstName() as string:
		return _fname

	def dump():
		pass
		# print("ID: ${_id}\nFirst Name: ${_fname}")

p = Person("1111", "Bamboo")
p.dump()
