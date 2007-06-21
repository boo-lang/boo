def map(items, function as callable):
	for item in items:
		yield function(item)
		
x2 = { value as int | return value*2 }
e = map(range(1, 4), x2).GetEnumerator()

assert e.MoveNext() and (2 == e.Current)
assert e.MoveNext() and (4 == e.Current)
assert e.MoveNext() and (6 == e.Current)
assert not e.MoveNext()
