"""
"""
interface IFoo:
	def Bar()
		
# no warnings for abstract class that dont
# provide implementation for all abstract
# members (that's what an abstract class is)
abstract class Foo(IFoo):
	pass
