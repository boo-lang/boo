"""
8
3
"""
import System

class GradeAttribute(Attribute):
	[property(Value)]
	_value as int
	
	def constructor():
		pass
		
	def constructor(value as int):
		_value = value
		
	static def GetGradeValue(type as System.Type):
		attribute as GradeAttribute = Attribute.GetCustomAttribute(type, GradeAttribute)
		return attribute.Value
	
[Grade(Value: 8)]
class Foo:
	pass
	
[Grade(Value: 3)]
class Bar:
	pass
	
print GradeAttribute.GetGradeValue(Foo)
print GradeAttribute.GetGradeValue(Bar)
