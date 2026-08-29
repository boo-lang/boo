"""
Foo.Name
Foo.Name
Bar.Name
Bar.Name
Zeng.Name
"""
class Foo:
	static Name:
		get:
			return "Foo.Name"
	
class Bar:
	static Name:
		get:
			return "Bar.Name"

class Baz:
	Name:
		get:
			return "Baz.Name"
			
class Zeng:
	Name = "Zeng.Name"


def pName(o as duck):
	print o.Name


pName(Foo)
pName(Foo())
pName(Bar)
pName(Bar())
pName(Zeng())
