#category FailsOnMono

def stackTrace(code as callable()):
	try:
		code()
	except x:
		return x.ToString()

s = stackTrace:
	cast(duck, 3).Foo()

// we expect to see line 5 and line 10 in there
assert 2 == /exceptions-3/.Matches(s).Count	
	
	
	

	
