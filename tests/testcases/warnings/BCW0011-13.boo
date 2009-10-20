"""
BCW0011-13.boo(11,16): BCW0011: WARNING: Type 'Consumer' does not provide an implementation for 'BaseInterface.Add(string)', a stub has been created.
"""
interface BaseInterface:
	def Add(s as string)

abstract class BaseAbstractClass:
	def Add(i as int):
		pass
		
class Consumer(BaseInterface, BaseAbstractClass):
	pass
