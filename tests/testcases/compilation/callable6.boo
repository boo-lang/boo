import NUnit.Framework

def foo():
	pass
	
f = foo
Assert.IsTrue(typeof(ICallable).IsAssignableFrom(f.GetType()), 
			"anonymous callable type implements ICallable")
