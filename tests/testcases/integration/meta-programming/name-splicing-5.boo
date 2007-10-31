"""
interface Bar:

	def bar()
"""
name = "bar"
type = [|
	interface $(name[:1].ToUpper() + name[1:]):
		
		def $name()
|]

print type.ToCodeString()


