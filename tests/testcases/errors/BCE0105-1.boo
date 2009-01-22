"""
BCE0105-1.boo(11,14): BCE0105: 'abstract' can only be applied to class, method, property and event definitions.
BCE0105-1.boo(16,23): BCE0105: 'abstract' can only be applied to class, method, property and event definitions.
BCE0105-1.boo(20,24): BCE0105: 'abstract' can only be applied to class, method, property and event definitions.
BCE0105-1.boo(23,17): BCE0105: 'abstract' can only be applied to class, method, property and event definitions.
BCE0105-1.boo(26,15): BCE0105: 'abstract' can only be applied to class, method, property and event definitions.
"""

abstract class Foo:

	abstract _name as string
	abstract Name:
		get:
			pass

	abstract callable Bar()

	abstract event Zeng as System.EventHandler

	abstract interface ISpam:
		pass

abstract struct FooStruct:
	public Bar as int

abstract enum FooEnum:
	Bar

