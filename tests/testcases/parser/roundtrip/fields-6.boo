"""
class Action:

	cb = null

	def constructor(callback):
		cb = callback

class A:

	[property(Go)]
	action = Action({ print('Hello World') })
"""
class Action:
	cb = null
	def constructor(callback):
		cb = callback

class A:
	[property(Go)]
	action = Action() def():
		print("Hello World")
