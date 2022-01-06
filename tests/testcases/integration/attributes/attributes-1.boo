"""
p

"""
namespace Test

import System
import System.Xml.Serialization from System.Private.Xml

[XmlRoot("p")]
class Person:
	pass
	
root as XmlRootAttribute = Attribute.GetCustomAttribute(Person, XmlRootAttribute)
print(root.ElementName)
