import NUnit.Framework

class Outer:

	class Inner:
		[property(Value)]
		_value

	[getter(InnerValue)]
	_innerValue = Inner(Value: "foo")

outer = Outer()
Assert.AreEqual("foo", outer.InnerValue.Value)
		
