"""
p

"""
namespace Test

using System
using System.Xml.Serialization from System.Xml

[XmlRoot("p")]
class Person:
	pass
	
asm = System.Reflection.Assembly.LoadWithPartialName("System.Xml")
personType = Type.GetType("Test.Person")
xmlRootType = asm.GetType("System.Xml.Serialization.XmlRootAttribute")
root as XmlRootAttribute = Attribute.GetCustomAttribute(personType, xmlRootType)
print(root.ElementName)
