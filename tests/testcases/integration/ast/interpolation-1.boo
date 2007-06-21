"""
def foo():
	print('Hello, world')
"""
invocation = ast { print("Hello, world") }
literal = ast:
	def foo():
		${invocation}
		
print literal.ToCodeString()
