import System.Xml from System.Xml

def selectElements(element as XmlElement, tagName as string):
	for node as XmlNode in element.ChildNodes:
		if node isa XmlElement and tagName == node.Name:
			yield node

xml = """
<document>
	<foo value='1' />
	<bar />
	<foo value='2' />
</document>
"""

document = XmlDocument()
document.LoadXml(xml)

for element in selectElements(document.DocumentElement, "foo"):
	print(element.GetAttribute("value"))
