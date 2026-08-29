"""
b
"""
import System

class CharAttribute(Attribute):
	public Value as char
	
[CharAttribute(Value: char('b'))]
class Attributed:
	pass
	
attribute as CharAttribute = Attribute.GetCustomAttribute(Attributed, CharAttribute)
print attribute.Value
