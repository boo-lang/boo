class Adder:

	public add = { value as int | return value*_amount }
	
	[property(Amount)]
	_amount = 1
	
f = Adder(Amount: 2)
assert 3 == f.add(3)/2
assert 5 == f.add(5)/2
	

