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
svn change log printer utility.

changelog.boo [FROM-REVISION [TO-REVISION]]

Example:
	changelog.boo PREV HEAD
	changelog.boo 123
	changelog.boo
"""
import System.Xml from System.Xml

class LogEntry:
	Author as string
	Date as date
	Message as string
	
	def constructor(element as XmlElement):
		Author = element.SelectSingleNode("author/text()").Value
		Date = date.Parse(element.SelectSingleNode("date/text()").Value)
		Message = element.SelectSingleNode("msg/text()").Value
	
	override def ToString():
		return "${Date} - ${Author}\n${Message}"
		
	static def Load(fromRevision, toRevision):
		doc = XmlDocument()
		doc.LoadXml(shell("svn", "log --xml -v -r ${fromRevision}:${toRevision}"))		
		return array(LogEntry(e) for e in doc.SelectNodes("//logentry"))
		
if len(argv) > 1:
	fromRevision, toRevision = argv
elif len(argv) > 0:
	fromRevision, = argv
	toRevision = "HEAD"
else:
	fromRevision = toRevision = "HEAD"
	
entries = LogEntry.Load(fromRevision, toRevision)
print join(entries, "\n")
					
