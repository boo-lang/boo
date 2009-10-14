"""
A.N1.Foo
A.N2.Bar
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.MetaProgramming
import Boo.Lang.Compiler.Ast

n1 = [|
	namespace A.N1
	
	class Foo:
		pass		
|]

n2 = [|
	namespace A.N2
	
	class Bar:
		pass
|]

main = [|
	namespace A
	
	import A.N1
	import A.N2
	
	print Foo()
	print Bar()
|]

compile(CompileUnit(n1, n2, main)).EntryPoint.Invoke(null, (null,))
