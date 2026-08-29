"""
FOO
BAR
"""
for s in System.IO.StringReader("foo\nbar"):
	print(s.ToUpper()) # s is declared as System.String
