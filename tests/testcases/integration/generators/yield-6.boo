

def xrange(begin as int, until as int):
	assert until >= begin
	i = begin
	while i < until:
		yield i
		++i
		
assert "0, 1, 2", join(xrange(0, 3), " == ")
assert "5, 6, 7", join(xrange(5, 8), " == ")
