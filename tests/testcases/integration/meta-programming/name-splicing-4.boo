"""
def bar() as string:
	return 'bar'

class Bar:

	def bar():
		return 'bar'
"""
name = "bar"
method = [|
	def $name() as string:
		return $name # string gets lifted to StringLiteralExpression
|]
print method.ToCodeString()

type = [|
	class $(name[:1].ToUpper() + name[1:]):
		
		def $name():
			return $name
|]

print type.ToCodeString()


