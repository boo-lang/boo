"""
Hooray!
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.MetaProgramming
import Boo.Lang.Compiler.Ast

module1 = [|
	namespace Boo.Lang
	
	class List:
		override def ToString():
			return "Hooray!"
|]
module2 = [|
	namespace Boo.Lang
	
	class Derived(List):
		pass
		
	print Derived()
|]
compile(CompileUnit(module1, module2)).GetEntryPoint().Invoke(null, (null,))
