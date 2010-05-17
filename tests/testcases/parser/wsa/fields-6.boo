"""
class Action:

	cb

	def constructor(callback):
		cb = callback

class A:

	[property(Go)]
	action = Action({ print('Hello World') })
"""
class Action:

	cb

	def constructor(callback):
		cb = callback
	end
end

class A:

	[property(Go)]
	action = Action({ print('Hello World') })
end

