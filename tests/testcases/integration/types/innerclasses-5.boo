import NUnit.Framework

class Outer:	

	[getter(Values)]
	_values as (Inner)

	class Inner:
		[property(Name)]
		_name
		
		def constructor(name):
			_name = name

	def constructor():
		_values = Inner("foo"), Inner("bar")

outer = Outer()
Assert.AreEqual(2, len(outer.Values))
Assert.AreEqual("foo", outer.Values[0].Name)
Assert.AreEqual("bar", outer.Values[1].Name)
		
