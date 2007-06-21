"""
BCE0105-1.boo(8,14): BCE0105: 'abstract' can only be applied to class, method, property and event definitions.
BCE0105-1.boo(14,23): BCE0105: 'abstract' can only be applied to class, method, property and event definitions.
BCE0105-1.boo(18,24): BCE0105: 'abstract' can only be applied to class, method, property and event definitions.
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
