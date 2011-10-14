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

assert fname is not null
assert lname is not null

attribute as XmlAttributeAttribute = Attribute.GetCustomAttribute(fname, XmlAttributeAttribute)
assert attribute is not null
assert "fname" == attribute.AttributeName


attribute = Attribute.GetCustomAttribute(lname, XmlAttributeAttribute)
assert attribute is not null
assert "lname" == attribute.AttributeName



