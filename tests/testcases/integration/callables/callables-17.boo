def foo():
	return "foo"
	
def bar():
	return 5
	
def hyphenate(fn as ICallable):
	return "-${fn()}"
	
def upper(fn as ICallable):
	return fn().ToString().ToUpper()
	
def test(expectedValues as List, decorator as ICallable):
	assert expectedValues[0] == decorator(foo)
	assert expectedValues[1] == decorator(bar)
	
test(["-foo", "-5"], hyphenate)
test(["FOO", "5"], upper)
