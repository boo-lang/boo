#category FailsOnMono
"""
m1 m1
m2 m2
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

def makeModule(name as string):
	m = [|
		import System
		
		[extension] def puts(this as string):
			print $name, this
			
		class $name:
			def run():
				$name.puts()
	|]
	m.Name = name
	return m
	
assembly = compile(CompileUnit(makeModule("m1"), makeModule("m2")))
for m in "m1", "m2":
	(assembly.GetType(m)() as duck).run()
