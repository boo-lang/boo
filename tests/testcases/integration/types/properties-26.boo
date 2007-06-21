class Foo:

	_gaga as int
	
	virtual gaga as int:
		get:
			return _gaga

class Bar(Foo):
	
	gaga as int:
		get:
			return 2*_gaga
		set:
			_gaga = value
			
b = Bar()
b.gaga = 4

assert 8 == b.gaga
assert 8 == (b as Foo).gaga
