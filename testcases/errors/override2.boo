"""
override2.boo(7,16): BCE0022: Cannot convert 'System.Int32' to 'System.String'.
override2.boo(10,32): BCE0072: Overriden method 'System.Object.ToString' has a return type of 'System.String' not 'System.Int32'. 
"""
class Foo:
	override def ToString():
		return 3
		
class Bar:
	override def ToString() as int:
		return 3
