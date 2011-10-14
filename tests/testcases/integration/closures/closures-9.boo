
value = 3
c = def:
	assert 3 == value
	value = 4 # change our local copy
	assert 4 == value
	
c()
assert 4 == value, "local variables must be shared"
