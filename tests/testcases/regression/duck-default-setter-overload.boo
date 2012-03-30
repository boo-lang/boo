"""
string: the string indexer value
object: the object indexer value
"""
[System.Reflection.DefaultMember("Item")]
class Container:
	Item(index as object):
		set:
			print "object: $value"

	Item(index as string):
		set:
			print "string: $value"

d = Container() as duck

str = "index"
obj = object()

d[str] = "the string indexer value"
d[obj] = "the object indexer value"