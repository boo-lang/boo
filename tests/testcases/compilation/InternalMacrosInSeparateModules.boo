"""
Hello
Hello
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

def run(*modules as (Module)):
	compileUnit = CompileUnit()
	for m in modules:
		compileUnit.Modules.Add(m.CloneNode());
	compile(compileUnit).EntryPoint.Invoke(null, (null,))

macros = [|
	namespace Test
	
	import Boo.Lang.PatternMatching
	
	macro say:
		case [| say $message |]:
			yield [| print $message |]
|]

client = [|
	namespace Test
	
	say "Hello"
|]

run(macros, client)
run(client, macros)
