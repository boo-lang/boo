class Person:

	_id as string
	_name as string

	def constructor(id as string, name as string):
		_id = id
		_name = name

	ID as string:
		get:
			return _id

	Name as string:
		get:
			return _name
		set:
			_name = value

c = Person("elvin", "Elvin Jones")
print(c.Name)
