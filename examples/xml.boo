#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

import System
import System.Xml.Serialization from System.Xml
import System.IO

class Address:
	public Street as string
	public Number as int
	
	override def ToString():
		return "${Number}, ${Street}"

class Person:
	
	_fname as string
	_lname as string
	_addresses as (Address)
	
	def constructor():
		pass
		
	def constructor(fname, lname):
		_fname = fname
		_lname = lname
		
	[XmlAttribute("FirstName")]
	FirstName:
		get:
			return _fname
		set:
			_fname = value
			
	[XmlAttribute("LastName")]
	LastName:
		get:
			return _lname
		set:
			_lname = value
			
	Addresses as (Address):
		get:
			return _addresses
		set:
			_addresses = value
			
p1 = Person("Homer", "Simpson")
p1.Addresses = (Address(Street: "Al. Foo", Number: 35),
				Address(Street: "Al.Bar", Number: 14))

buffer = StringWriter()
serializer = XmlSerializer(Person)
serializer.Serialize(buffer, p1)

Console.WriteLine(buffer.ToString())

p2 as Person = serializer.Deserialize(StringReader(buffer.ToString()))
Console.WriteLine("${p2.LastName}, ${p2.FirstName}")
for address in p2.Addresses:
		Console.WriteLine("\t${address}")




