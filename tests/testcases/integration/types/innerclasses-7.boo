namespace Foo.Bar

class Outer:	

	[getter(Values)]
	_values = (Inner("foo"), Inner("bar"))

	class Inner:
		[property(Name)]
		_name = ""
		
		def constructor(name):
			_name = name

outer = Outer()
assert 2 == len(outer.Values)
assert "foo" == outer.Values[0].Name
assert "bar" == outer.Values[1].Name
		
