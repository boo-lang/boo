class Foo:
	def bar():
		baz()
		
	def baz():
		raise "hit me"

def stackTrace(code as callable()):
	try:
		code()
	except x:
		return firstLines(x).Trim()

def firstLines(o):
	return join(/\n/.Split(o.ToString())[:3], "\n")
	
se = stackTrace({ Foo().bar() })
de = stackTrace({ (Foo() as duck).bar() })
assert se == de, "'${se}' != '${de}'" 
	
	

	
