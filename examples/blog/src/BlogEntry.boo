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
		
	Entries as (BlogEntry):
		get:
			return _entries.ToArray(BlogEntry)
	
	[Query]
	def GetLatestEntries(count as int):
		return _entries[:count]

	[Query]
	def GetEntriesPostedAt(postingDate as date):
		return [entry for entry as BlogEntry in _entries
				if entry.DatePosted.Date == postingDate]
