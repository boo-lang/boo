import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

[Extension]
def WithLocation[of T(Node)](node as T, location as LexicalInfo):
	node.LexicalInfo = location
	return node
	
location = LexicalInfo("foo.boo", 1, 1)
code = [| foo |].WithLocation(location)
assert code.Name == "foo"
assert code.LexicalInfo is location
