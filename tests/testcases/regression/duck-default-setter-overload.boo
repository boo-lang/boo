[System.Reflection.DefaultMember("Item")]
class Container:
	private _value as object

	Item(index as object):
		get:
			return _value
		set:
			_value = value

	Item(index as string):
		get:
			return _value
		set:
			_value = value


d = Container() as duck

str = "index"
obj = Object()

d[str] = "the string indexer value"
d[obj] = "the object indexer value"