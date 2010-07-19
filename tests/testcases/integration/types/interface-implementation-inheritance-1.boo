"""
FOO
"""
class StringList(List[of string], string*):
	pass
	
ss = StringList()
ss.Add("foo")
for s in ss:
	print s.ToUpper()
