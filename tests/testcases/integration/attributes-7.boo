"""
8.5
3.2
5
"""
import System

class GradeAttribute(Attribute):
	[property(Value)]
	_value as single
	
	def constructor():
		pass
		
	def constructor(value as int):
		_value = value
		
	static def GetGradeValue(type as System.Type):
		attribute as GradeAttribute = Attribute.GetCustomAttribute(type, GradeAttribute)
		return attribute.Value
	
[Grade(Value: 8.5)]
class Foo:
	pass
	
[Grade(Value: 3.2)]
class Bar:
	pass
	
[Grade(Value: 5)]
class Baz:
	pass
	
def printGrade(t as Type):
	value = GradeAttribute.GetGradeValue(t)
	print value.ToString(System.Globalization.CultureInfo.InvariantCulture)
	
printGrade(Foo)
printGrade(Bar)
printGrade(Baz)
