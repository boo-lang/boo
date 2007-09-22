
import Boo.Lang.Compiler.MetaProgramming

dsl = [|

	namespace PetDSL

	import Boo.Lang.Compiler.MetaProgramming
	import Boo.Lang.Compiler.Ast
	
	class Pet:
		public name = "I need a name"
	
	[meta] def onCreate(reference as ReferenceExpression, block as BlockExpression):
		return [|
			$reference = $(pascalCase(reference))()
			$(block.Body)
		|]
		
	def pascalCase(r as ReferenceExpression):
		return ReferenceExpression(Name: pascalCase(r.Name))
		
	def pascalCase(s as string):
		return s[:1].ToUpper() + s[1:]

|]

app = [|
	import PetDSL
	
	onCreate pet:
		print pet.name
|]


asm = compile(app, compile(dsl))
asm.EntryPoint.Invoke(null, (null,))
