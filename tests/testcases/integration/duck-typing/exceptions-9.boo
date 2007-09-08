class Foo:
	bar[i]:
		set:
			self.baz = i
		
	baz as object:
		set:
			raise "hit me"

def stackTrace(code as callable()):
	try:
		code()
	except x:
		return firstLines(x)

def firstLines(o):
	return join(/\n/.Split(o.ToString())[:3], "\n").Trim()
	
se = stackTrace({ Foo().bar[42] = "foo" })
de = stackTrace({ (Foo() as duck).bar[42] = "foo" })
assert se == de, "'${se}' != '${de}'"
