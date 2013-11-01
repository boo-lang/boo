

def xrange(begin as int, end as int):
	assert end >= begin
	i = begin
	while i < end:
		yield i
		++i
		
assert "0, 1, 2", join(xrange(0, 3), " == ")
assert "5, 6, 7", join(xrange(5, 8), " == ")
