
def foo():
	return "it works!"
	
fn as object
fn = foo

assert System.MulticastDelegate is fn.GetType().BaseType
assert "it works!" == cast(ICallable, fn)()
