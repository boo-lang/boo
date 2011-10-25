

callable StringFunction() as string

def foo():
	return "foo"

a = foo
assert "foo" == a()

fn as StringFunction
fn = a
assert StringFunction is fn.GetType()
assert "foo" == fn()
