import NUnit.Framework

class Person:
	[property(Name)]
	_name as string
	def constructor(name):
		_name = name

p = Person("John") // static typing
d as duck = p // dynamic typing

Assert.AreSame(p, d, "being duck does not change the reference")
Assert.IsTrue(d isa Person, "d isa Person")
Assert.AreEqual(p.Name, d.Name, "p.Name")

d.Name = "Eric"
Assert.AreEqual("Eric", p.Name)
Assert.AreSame(p.Name, d.Name)





