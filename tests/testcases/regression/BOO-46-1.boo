import NUnit.Framework

class Foo:

	_prefix = null
	
	def constructor(prefix):
		_prefix = prefix

	def call(c as ICallable):
		return c()

	def foo():
		return "${_prefix} - foo"

	def run():
		return call(foo)

Assert.AreEqual("bar - foo", Foo("bar").run())
