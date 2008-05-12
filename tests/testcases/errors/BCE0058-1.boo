"""
BCE0058-1.boo(7,15): BCE0058: 'self' is not valid in a static method, static property, or static field initializer.
BCE0058-1.boo(10,15): BCE0058: 'self' is not valid in a static method, static property, or static field initializer.
BCE0058-1.boo(12,10): BCE0058: 'self' is not valid in a static method, static property, or static field initializer.
BCE0058-1.boo(16,20): BCE0058: 'self' is not valid in a static method, static property, or static field initializer.
"""
static class Foo:
	def constructor():
		print self

	def bar():
		print self

	_x = self

	Prop:
		get:
			return self
