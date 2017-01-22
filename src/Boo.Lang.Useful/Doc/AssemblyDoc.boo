#region license
// Copyright (c) 2013 by Harald Meyer auf'm Hofe (harald.meyer@users.sourceforge.net)
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

namespace Boo.Lang.Useful.Doc

import System
import System.Xml
import System.Collections
import System.Reflection
import System.Globalization.CultureInfo

class AssemblyDoc:
"""
	The documentation of an assembly in Sandcastle style.
"""
	public def constructor(fileName as string):
		_fileName = fileName
		self.Load(fileName)
	
	[Getter(FileName)]
	_fileName as string
	"""
	This is the name of the documentation file like
	'C:\\Programme\\Reference Assemblies\\Microsoft\\Framework\\v3.5\\System.Core.xml'.
	"""
	
	[Getter(AssemblyName)]
	_assemblyName as string
	"""
	The name of the documented assembly like 'System.Core'.
	"""
	
	_members = Generic.SortedList[of string, XmlNodeList]()
	
	def Load(fileName as string):
	"""
	Loads documentation from a certain file.
	"""
		doc=XmlDocument()
		doc.Load(fileName)
		self._assemblyName = doc.GetElementsByTagName("assembly")[0].InnerText
		memberlist = doc.GetElementsByTagName("member")
		for m as XmlNode in memberlist:
			n=m.Attributes.GetNamedItem("name").InnerText
			self._members[n] = m.ChildNodes
	
	def Find(designator as string):
		result as XmlNodeList
		if self._members.TryGetValue(designator, result):
			return result
		return null
	