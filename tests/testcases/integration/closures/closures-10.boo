import NUnit.Framework

class Factory:

	_prefix
	
	def constructor(prefix):
		_prefix = prefix
		
	def Create(suffix):
		return { item | return "${_prefix}${item}${suffix}" }
		
c1 = Factory("<").Create(">")
c2 = Factory("{{").Create("}}")

Assert.AreEqual("<Yellow!>", c1("Yellow!"))
Assert.AreEqual("{{Zeng!}}", c2("Zeng!"))
