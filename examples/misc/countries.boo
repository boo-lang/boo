#region license
// Copyright (c) 2003, 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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

class Country:
	public Name as string
	public Hex as string

	override def ToString():
		return "${Name} - ${Hex}"

# an array of Country objects just to show how XmlSerializer
# handles it

countries = (
			Country(Name: "Foo", Hex: "0x00"),
			Country(Name: "Bar", Hex: "0x01"))
			
# boo array types are defined with (ElementType)

serializer = XmlSerializer(typeof((Country)), XmlRootAttribute("Countries"))
serializer.Serialize(Console.Out, countries)


# now let's read the file

using stream=File.OpenRead("countries.xml"):	
	countries = serializer.Deserialize(stream)
	print("${len(countries)} countries deserialized.")
	
	# print first 3 countries just because we can :-)
	print(join(countries[:3], "\n"))


