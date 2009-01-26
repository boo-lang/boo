"""
Hello World
"""
class Action:
	cb = null
	def constructor(callback):
		cb = callback
		
	def go():
		cast(callable, cb)()
		
class A:
	action = Action({ print("Hello World"); })
	
	def go():
		action.go()
		
A().go()
	




