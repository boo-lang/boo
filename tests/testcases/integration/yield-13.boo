def generator() as System.Collections.IEnumerable:
	yield 1
	yield "um"
	
assert "1: um" == join(generator(), ": ")
