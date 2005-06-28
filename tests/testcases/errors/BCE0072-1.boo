"""
BCE0072-1.boo(5,32): BCE0072: Overriden method 'System.Object.ToString' has a return type of 'System.String' not 'System.Int32'.
"""
class Bar:
	override def ToString() as int:
		return 3
