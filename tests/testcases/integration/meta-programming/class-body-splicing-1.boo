"""
class Foo:

	override def ToString():
		pass
"""
body = [|
	override def ToString():
		pass
|]
className = "Foo"
type = [|
	class $className:
		$body
|]
print type.ToCodeString()


