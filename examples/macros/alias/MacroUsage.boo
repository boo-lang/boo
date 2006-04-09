def foo(ref paramWithReallyLongName as int, j):
	
	alias paramWithReallyLongName as i
	i = 3*i
	
	if false:
		alias i as j
		print j
		
	print j # won't be replaced
		
	
i = 4
foo(i, "bar")
print i
