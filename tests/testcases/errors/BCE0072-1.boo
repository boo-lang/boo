"""
BCE0072-1.boo(5,32): BCE0072: Overriden method 'object.ToString' has a return type of 'string' not 'int'.
"""
class Bar:
	override def ToString() as int:
		return 3
