"""
"""
interface BaseInterface:
	def Add(s as string)

abstract class BaseAbstractClass:
	def Add(i as string):
		pass
		
class Consumer(BaseInterface, BaseAbstractClass):
	pass
