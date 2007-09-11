"""
internal class Foo(Bar):

	def constructor():
		pass

	def run(message as string):
		print message
"""
literal = [|
	internal class Foo(Bar):
		def constructor():
			pass
		def run(message as string):
			print message
|]

assert literal isa Boo.Lang.Compiler.Ast.ClassDefinition
print literal.ToCodeString()
