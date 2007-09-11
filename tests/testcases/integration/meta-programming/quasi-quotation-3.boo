"""
import System

class Foo:
	pass

class Bar:
	pass
"""
import Boo.Lang.Compiler.Ast

m = [|	
	class Foo:
		pass
		
	class Bar:
		pass
|]

assert m isa Module
m.Imports.Add([| import System |])
print m.ToCodeString()
