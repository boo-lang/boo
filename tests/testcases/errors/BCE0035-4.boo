"""
BCE0035-4.boo(8,5): BCE0035: 'Class.Foo' conflicts with inherited member 'IClass.Foo'.
"""
interface IClass:
	event Foo as System.EventHandler

class Class (IClass):
	Foo as string:
		get: return "I'm not an event"

