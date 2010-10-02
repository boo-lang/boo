"""
class Foo:
	pass
"""
className = "Foo"
type = [|
	class $className:
		pass
|]
print type.ToCodeString()


