"""
homer

"""
class Person:
	override def ToString():
		return Name
		
	Name:
		get:
			return _name
		set:
			_name = value
		
	_name as string
		
System.Console.WriteLine(Person(Name: "homer"))
