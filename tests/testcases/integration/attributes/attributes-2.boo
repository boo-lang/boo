"""
p
Foo.Bar
"""
import System
import System.Xml.Serialization from System.Xml

[XmlRoot("p", Namespace: "Foo.Bar")]
class Person:
	pass
	
xmlroot as XmlRootAttribute = Attribute.GetCustomAttribute(Person, XmlRootAttribute)
print(xmlroot.ElementName)
print(xmlroot.Namespace)
