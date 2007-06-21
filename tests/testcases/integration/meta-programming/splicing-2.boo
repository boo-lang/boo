"""
def foo():
	print('Hello, world')
"""
literal = [|
	def foo():
		// splice(qq(x)) => x
		$([| print("Hello, world") |])
|]

print literal.ToCodeString()
