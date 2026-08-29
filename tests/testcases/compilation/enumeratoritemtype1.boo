"""
FOO
BAR
"""
[EnumeratorItemType(string)]
class Enumerable(System.Collections.IEnumerable):
	def GetEnumerator():
		return ("foo", "bar").GetEnumerator()
		
for s in Enumerable():
	print(s.ToUpper()) # s is declared as System.String
