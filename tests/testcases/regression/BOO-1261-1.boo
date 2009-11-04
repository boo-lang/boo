"""
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

def test(*modules as (Module)):
	compileUnit = CompileUnit()
	for m in modules:
		compileUnit.Modules.Add(m.CloneNode());
	assert compile(compileUnit).GetType("A.Delta") is not null

m1 = [|
	namespace B
	
	class Basic(A.Delta):
		pass
|]

m2 = [|
	namespace A
	
	interface IBase:
		pass
	
	class Delta(IBase):
		pass
|]

test(m1, m2)
#test(m2, m1)
