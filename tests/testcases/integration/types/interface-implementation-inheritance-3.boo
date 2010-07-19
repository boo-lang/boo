"""
foo
bar
"""
interface IDocument(System.Collections.Generic.IList[of string]):
	AllText as string:
		get
		
class StringDocument(List[of string], IDocument):
	AllText:
		get: return join(self, System.Environment.NewLine)
		
def printAllText(d as IDocument):
	print d.AllText
	
d = StringDocument()
d.Add("foo")
d.Add("bar")
printAllText d
