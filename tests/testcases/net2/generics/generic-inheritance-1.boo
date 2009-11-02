class xList[of T](List[of T]):
	pass
	
l = xList[of int]()
l.Add(42)

assert 42 == l[0]
assert 42 == l[-1]
