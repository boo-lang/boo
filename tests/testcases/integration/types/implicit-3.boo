"""
Alpha(1)
Bravo(1)
Alpha(1)
Bravo(1)
"""

class Alpha:
	def constructor(value as double):
		_value = value
	
	override def ToString():
		return "Alpha(${_value})"
	
	[getter(Value)] _value as double

class Bravo:
	def constructor(value as double):
		_value = value
	
	def op_Implicit(value as Alpha) as Bravo:
		return Bravo(value.Value)
	
	def op_Implicit(value as Bravo) as Alpha:
		return Alpha(value.Value)
	
	override def ToString():
		return "Bravo(${_value})"
	
	[getter(Value)] _value as double

def PrintAlpha(alpha as Alpha):
	print alpha
	
def PrintBravo(bravo as Bravo):
	print bravo

PrintAlpha(Alpha(1))
PrintBravo(Bravo(1))
PrintAlpha(Bravo(1))
PrintBravo(Alpha(1))

