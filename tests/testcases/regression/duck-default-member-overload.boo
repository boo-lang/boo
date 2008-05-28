"""
Hello
"""
import System.Xml

doc = XmlDocument()
	
data = """
<identity>
<avatar>
<name>Hello</name>
</avatar>
</identity>
"""

doc.LoadXml(data)

identitiesList = doc.GetElementsByTagName("identity")
for n as duck in identitiesList:
	print n["avatar"]["name"].InnerText 
