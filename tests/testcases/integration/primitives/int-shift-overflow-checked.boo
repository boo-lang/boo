"""
2147483648
OverflowException
"""
checked:
	try:
		l = 1L<<31
		print l
		i as int = l
		print i
	except x:
		print x.GetType().Name
