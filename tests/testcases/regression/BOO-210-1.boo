struct Test:
	
	public i as int
	
	def constructor(j as int):
		i = j
		
	def Add(j as int) as Test:
		i += j
		return self

ix = Test(0)
assert 1 == ix.Add(1).i
