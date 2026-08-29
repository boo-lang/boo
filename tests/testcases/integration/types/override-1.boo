"""
boo
"""
class Language:

	_name as string
	
	def constructor(name as string):
		_name = name
		
	def ToString() as string:
		return _name
		
System.Console.Write(Language("boo"))

