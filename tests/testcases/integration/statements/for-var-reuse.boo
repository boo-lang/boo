"""
FOO
BAR
bar
"""
def test(items as object*):
	s as string
	for s in items:
		print s.ToUpper()
	print s

test(["foo", "bar"])
