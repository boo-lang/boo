import System.Reflection
import NUnit.Framework

class Person:

	_fname = ""
	
	FirstName as string:
		get:
			return _fname
		virtual set:
			_fname = value
			
p = typeof(Person).GetProperty("FirstName")
getter = p.GetGetMethod()
setter = p.GetSetMethod()

Assert.IsNotNull(getter, "getter.IsNotNull")
Assert.IsTrue(getter.IsPublic, "getter.IsPublic")
Assert.IsFalse(getter.IsVirtual, "not getter.IsVirtual")

Assert.IsNotNull(setter, "setter.IsNotNull")
Assert.IsTrue(setter.IsPublic, "setter.IsPublic")
Assert.IsTrue(setter.IsVirtual, "setter.IsVirtual")

