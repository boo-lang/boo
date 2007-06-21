a = [5, 4, 3, 2, 1]
	
assert 0 == a.IndexOf({ item as int | return item > 3 })
assert 3 == a.IndexOf({ item as int | return item < 3 })
