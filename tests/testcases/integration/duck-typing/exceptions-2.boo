import Boo.Lang.Runtime
import My

def stackTrace(code as callable()):
	try:
		code()
	except x:
		return firstLines(x.InnerException or x)

def firstLines(o):
	return join(/\n/.Split(o.ToString())[:3], "\n").Trim()
	
class My:
	[extension] static def to_s(a as System.Array):
		foo(a)
		
	static def foo(o):
		raise "Not implemented"
			
RuntimeServices.WithExtensions(My):	
	se = stackTrace({ (1, 2, 3).to_s() })
	de = stackTrace({ ((1, 2, 3) as duck).to_s() })
	assert se == de, "'${se}' != '${de}'" 
	
