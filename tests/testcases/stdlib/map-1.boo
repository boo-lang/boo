result = map([1, 2, 3]) do (item as int):
	return item*2
	
assert result isa System.Collections.IEnumerable
a, b, c = result
assert 2 == a
assert 4 == b
assert 6 == c

assert "2 4 6" == join(result)
