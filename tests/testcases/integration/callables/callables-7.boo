import NUnit.Framework

def foo():
	pass
	
f = foo
Assert.IsTrue(f isa ICallable,
			"anonymous callable type implements ICallable")
