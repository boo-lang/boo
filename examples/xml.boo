#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
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




