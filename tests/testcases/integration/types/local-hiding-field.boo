"""
42 21
"""
class Foo:
	value = 42
	
	def Bar():
		oldValue = value
		value as int = 21
		print oldValue, value
		
Foo().Bar()
