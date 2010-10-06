"""
class C:

	private field

	property:
		get:
			pass

	def method(arg):
		pass
"""
import Boo.Lang.Compiler.Ast

type as TypeReference

code = [|
	class C:
		private field as $type
		property as $type:
			get: pass
		def method(arg as $type) as $type:
			pass
|]

print code.ToCodeString()
