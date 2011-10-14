

def foo():
	return "foo"
	
def bar():
	return "bar"
	
for expected, fn as ICallable in zip(["foo", "bar"], [foo, bar]):
	assert expected == fn()
