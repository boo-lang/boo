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


"""
This example shows how to use IQuackFu.QuackGet to provide
transparent access to xml elements.
"""

import System
import System.Xml from System.Xml

class XmlObject(IQuackFu):
	
	_element as XmlElement
	
	def constructor(element as XmlElement):
		_element = element
		
	def constructor(text as string):
		doc = XmlDocument()
		doc.LoadXml(text)
		_element = doc.DocumentElement
		
	def QuackInvoke(name as string, args as (object)) as object:
		pass
		
	def QuackSet(name as string, value) as object:
		pass
		
	def QuackGet(name as string) as object:
		element = _element.SelectSingleNode(name)
		return XmlObject(element) if element is not null
		
	override def ToString():
		return _element.InnerText
		
		
xml = """
<Person>
	<FirstName>John</FirstName>
	<LastName>Cleese</LastName>
</Person>
"""

person as duck = XmlObject(xml)
print person.FirstName
print person.LastName
		
		

