#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
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




