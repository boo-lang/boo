
def echo(message):
	return message
	
def call(fn as ICallable, arg):
	return fn(arg)

fn = echo
assert "boo" == fn("boo")
assert "rulez!" == call(fn, "rulez!")
