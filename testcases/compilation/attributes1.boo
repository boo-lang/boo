"""
p
Foo.Bar
Employee

"""
import System
import System.Xml.Serialization from System.Xml

[XmlRoot("p", Namespace: "Foo.Bar")]
[XmlInclude(Employee)]
class Person:
	pass
	
class Employee(Person):
	pass
	
xmlroot as XmlRootAttribute = Attribute.GetCustomAttribute(Person, XmlRootAttribute)
print(xmlroot.ElementName)
print(xmlroot.Namespace)

xmlinc as XmlIncludeAttribute = Attribute.GetCustomAttribute(Person, XmlIncludeAttribute)
print(xmlinc.Type.Name)
