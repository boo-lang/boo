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
					
