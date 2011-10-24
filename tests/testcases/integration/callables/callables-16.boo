

def foo():
	return "foo"
	
def bar():
	return "bar"
	
i = -1
expected = "foo", "bar"
for f in foo, bar:
	assert expected[++i] == f()
