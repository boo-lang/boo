import System.Reflection
import NUnit.Framework

class Person:

	_fname = ""
	
	virtual FirstName as string:
		get:
			return _fname
		set:
			_fname = value
			
p = typeof(Person).GetProperty("FirstName")
getter = p.GetGetMethod()
setter = p.GetSetMethod()

Assert.IsNotNull(getter, "getter.IsNotNull")
Assert.IsTrue(getter.IsPublic, "getter.IsPublic")
Assert.IsTrue(getter.IsVirtual, "getter.IsVirtual")

Assert.IsNotNull(setter, "setter.IsNotNull")
Assert.IsTrue(setter.IsPublic, "setter.IsPublic")
Assert.IsTrue(setter.IsVirtual, "setter.IsVirtual")

