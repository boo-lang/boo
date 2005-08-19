"""
def foo():
	print('Hello, world')
"""
literal = ast:
	def foo():
		print("Hello, world")
		
print literal.ToCodeString()
