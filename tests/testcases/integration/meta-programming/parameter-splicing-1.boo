"""
def foo(bar as int):
	pass
"""

name = "bar"
method = [|
	def foo($name as int):
		pass
|]
print method.ToCodeString()
