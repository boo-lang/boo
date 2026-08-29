"""
def foo(bar as int):
	pass
"""

name = "bar"
type = "int"
method = [|
	def foo($name as $type):
		pass
|]
print method.ToCodeString()
