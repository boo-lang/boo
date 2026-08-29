"""
def foo():
	print('Hello, world')
"""
invocation = [| print("Hello, world") |]
literal = [|
	def foo():
		$invocation
|]

print literal.ToCodeString()
