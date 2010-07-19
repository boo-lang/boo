"""
foo
bar
"""
import Boo.Lang.Compiler.MetaProgramming

interface IDocument(System.Collections.Generic.IList[of string]):
	AllText as string:
		get
		
code = [|
	import System
	
	class StringDocument(List[of string], IDocument):
		AllText:
			get: return join(self, Environment.NewLine)
			
	def printAllText(d as IDocument):
		print d.AllText
		
	d = StringDocument()
	d.Add("foo")
	d.Add("bar")
	printAllText d
|]

compile(code, typeof(IDocument).Assembly).EntryPoint.Invoke(null, (null,))
