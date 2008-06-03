"""
BCE0158-1.boo(17,9): BCE0158: Cannot invoke instance method 'BeforeInit' before object initialization. Move your call after 'self' or 'super'.
BCE0158-1.boo(19,15): BCE0158: Cannot invoke instance method 'BeforeInitBase' before object initialization. Move your call after 'self' or 'super'.
BCE0158-1.boo(20,9): BCE0158: Cannot invoke instance method 'BeforeInitBase' before object initialization. Move your call after 'self' or 'super'.
"""

class Base:
	def BeforeInitBase():
		pass

class Derived(Base):
	def constructor():
		pass

	def constructor(x as int):
		object().ToString()
		BeforeInit()
		StaticBeforeInitWeDoNotCareAbout()
		super.BeforeInitBase()
		BeforeInitBase()
		self()
		StaticAfterInit()
		AfterInit()

	def BeforeInit():
		pass

	static def StaticBeforeInitWeDoNotCareAbout():
		pass

	def AfterInit():
		pass

	static def StaticAfterInit():
		pass

