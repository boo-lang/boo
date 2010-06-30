class Foo:
	bar:
		set:
			self.baz = value
		
	baz as object:
		set:
			raise "hit me"

def stackTrace(code as callable()):
	try:
		code()
	except x:
		return firstLines(x.InnerException or x)

def firstLines(o):
	return join(/\n/.Split(o.ToString())[:3], "\n").Trim()
	
se = stackTrace({ Foo().bar = "foo" })
de = stackTrace({ (Foo() as duck).bar = "foo" })
assert se == de, "'${se}' != '${de}'" 
	
	

	
