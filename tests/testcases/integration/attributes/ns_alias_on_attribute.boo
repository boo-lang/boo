import System.Xml.Serialization as xs

class Class1:

	[System.Xml.Serialization.XmlIgnoreAttribute]
	A:
		get:
			return "A!"

	[xs.XmlIgnoreAttribute]
	B:
		get:
			return "B!"

	[xs.XmlIgnore]
	C:
		get:
			return "C!"

c = Class1()
assert "A!" == c.A
assert "B!" == c.B
assert "C!" == c.C
