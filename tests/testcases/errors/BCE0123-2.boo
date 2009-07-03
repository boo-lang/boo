"""
BCE0123-2.boo(12,4): BCE0123: Invalid generic parameter type 'void'.
BCE0123-2.boo(13,11): BCE0123: Invalid generic parameter type 'void'.
"""

class Foo[of T]:
	pass
class Bar:
	def Haha[of T]() as T:
		pass

Foo[of void]()
Bar().Haha[of void]()

