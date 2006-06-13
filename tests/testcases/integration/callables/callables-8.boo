import NUnit.Framework

def echo(message):
	return message
	
def call(fn as ICallable, arg):
	return fn(arg)

fn = echo
Assert.AreEqual("boo", fn("boo"))
Assert.AreEqual("rulez!", call(fn, "rulez!"))
