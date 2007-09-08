class Foo:
	bar:
		get:
			return Bar()
		
class Bar:
	self[i]:
		get:
			return baz()
	def baz() as object:
		raise "hit me"

def stackTrace(code as callable()):
	try:
		code()
	except x:
		return firstLines(x)

def firstLines(o):
	return join(/\n/.Split(o.ToString())[:3], "\n").Trim()
	
se = stackTrace({ print Foo().bar[42] })
de = stackTrace({ print((Foo() as duck).bar[42]) })
assert se == de, "'${se}' != '${de}'"
