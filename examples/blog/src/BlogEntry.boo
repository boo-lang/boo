namespace BooLog

import System
import Bamboo.Prevalence.Attributes

class BlogEntry:
	
	[getter(ID)]
	_id = Guid.NewGuid()
	
	[getter(DatePosted)]
	_datePosted = date.Now
	
	[getter(Title)]
	_title as string
	
	[getter(Body)]
	_body as string
	
	def constructor([required] title, [required] body):
		_title = title
		_body = body
	
class BlogSystem(MarshalByRefObject):
	
	_entries = []
	
	def Post([required] entry as BlogEntry):
		_entries.Insert(0, entry)
	
	[Query]
	def GetLatestEntries(count as int):
		return _entries[:count]

	[Query]
	def GetEntriesPostedAt(postingDate as date):
		return [entry for entry as BlogEntry in _entries
				if entry.DatePosted.Date == postingDate]
