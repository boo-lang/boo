class Foo:
	bar[i]:
		get:
			return self.baz
		
	baz as object:
		get:
			raise "hit me"

def stackTrace(code as callable()):
	try:
		code()
	except x:
		return firstLines(x.InnerException or x)

def firstLines(o):
	return join(/\n/.Split(o.ToString())[:3], "\n").Trim()
	
se = stackTrace({ print Foo().bar[42] })
de = stackTrace({ print((Foo() as duck).bar[42]) })
assert se == de, "'${se}' != '${de}'"
