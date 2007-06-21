import NUnit.Framework
import System
import System.Xml.Serialization from System.Xml

class Customer:

	[XmlAttribute("fname")]
	public FirstName as string
	
	[XmlAttribute("lname")]
	public LastName as string
	
type = Customer
fname = type.GetField("FirstName")
lname = type.GetField("LastName")

Assert.IsNotNull(fname, "FirstName")
Assert.IsNotNull(lname, "LastName")

attribute as XmlAttributeAttribute = Attribute.GetCustomAttribute(fname, XmlAttributeAttribute)
Assert.IsNotNull(attribute, "Custom attribute not found on field FirstName!")
Assert.AreEqual("fname", attribute.AttributeName)


attribute = Attribute.GetCustomAttribute(lname, XmlAttributeAttribute)
Assert.IsNotNull(attribute, "Custom attribute not found on field LastName!")
Assert.AreEqual("lname", attribute.AttributeName)



