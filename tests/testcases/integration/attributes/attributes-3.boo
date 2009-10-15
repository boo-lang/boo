"""
DerivedClass
"""
import System
import System.Xml.Serialization from System.Xml
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

class C:
	[XmlInclude(DerivedClass)]	
	def GetItem() as BaseClass:
		pass

GetItem = typeof(C).GetMethod("GetItem")
xmlinc as XmlIncludeAttribute = Attribute.GetCustomAttribute(GetItem, XmlIncludeAttribute)
print(xmlinc.Type.Name)
