"""
FOO
BAR
"""
[EnumeratorItemType(string)]
class Enum(System.Collections.IEnumerable):
	def GetEnumerator():
		return ("foo", "bar").GetEnumerator()
		
for s in Enum():
	print(s.ToUpper()) # s is declared as System.String
