"""
DerivedClass
"""
import System
import System.Xml.Serialization from System.Xml
import Boo.Lang.Compiler.Tests from Boo.Lang.Compiler.Tests

class C:
	[XmlInclude(DerivedClass)]	
	def GetItem() as BaseClass:
		pass

GetItem = typeof(C).GetMethod("GetItem")
xmlinc as XmlIncludeAttribute = Attribute.GetCustomAttribute(GetItem, XmlIncludeAttribute)
print(xmlinc.Type.Name)
