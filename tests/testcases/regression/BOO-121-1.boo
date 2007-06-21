class Attributes:
	
	_attributes = []

	Item(index as int):
		get:
			return _attributes[index]

	Item(name as string):
		get:
			return [attr for attr as Attribute
					in _attributes
					if name == attr.Name]

	def Add([required] attr as Attribute):
		_attributes.Add(attr)

class Attribute:
	[property(Name)]
	_name = ""

a1 = Attribute(Name: "foo")
a2 = Attribute(Name: "foo")
a3 = Attribute(Name: "bar")

attributes = Attributes()
attributes.Add(a1)
attributes.Add(a2)
attributes.Add(a3)

assert attributes.Item["foo"] == [a1, a2]
assert attributes.Item["bar"] == [a3]
assert attributes.Item[0] == a1
assert attributes.Item[-1] == a3
