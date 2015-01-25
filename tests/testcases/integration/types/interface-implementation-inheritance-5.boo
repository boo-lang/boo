#category FailsOnMono
"""
foo
bar
"""
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

interface IDocument(System.Collections.Generic.IList[of string]):
	AllText as string:
		get
		
class StringList(List[of string]):
	pass
		
preservingLexicalInfo:
	code = [|
		import System
		
		class StringDocument(StringList, IDocument):
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
