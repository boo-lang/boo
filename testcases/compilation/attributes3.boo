"""
All
A class
"""
import System

[AttributeUsage(AttributeTargets.All)]
class DocumentationAttribute(Attribute):
	
	[getter(Text)]
	_text as string
	
	def constructor([required]text as string):
		_text = text
		
[Documentation("A class")]
class TargetClass:
	pass
	

usage as AttributeUsageAttribute = Attribute.GetCustomAttribute(DocumentationAttribute, AttributeUsageAttribute)
print(usage.ValidOn)

attribute as DocumentationAttribute = Attribute.GetCustomAttribute(TargetClass, DocumentationAttribute)
print(attribute.Text)
