
def foo():
	pass
	
f = foo
assert f isa ICallable, "anonymous callable type implements ICallable"
