"""
nested of a class with dtor
"""
//BOO-1059

class A:
	def destructor():
		pass

	private class B (System.Exception):
		def constructor(message as string):
			super(message)

	def Raise():
		print B("nested of a class with dtor").Message

A().Raise()

