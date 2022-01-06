"""
bar
"""
import System.Xml

doc = XmlDocument()
a = doc.CreateAttribute("foo")
a.InnerText = "bar"
print a.InnerText
