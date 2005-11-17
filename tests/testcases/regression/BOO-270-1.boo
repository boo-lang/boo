"""
Class, Struct, Enum
True
"""
import System

[AttributeUsage(AttributeTargets.Enum|AttributeTargets.Class|AttributeTargets.Struct, AllowMultiple: true)]
class Author(Attribute):
	pass
	

usage as AttributeUsageAttribute = Attribute.GetCustomAttributes(Author, AttributeUsageAttribute)[0]
print usage.ValidOn
print usage.AllowMultiple
