#region license
// Copyright (c) 2026 the Boo contributors
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

namespace Boo.Lang.Lsp.Workspace

import System.Collections.Generic

class DocumentStore:
"""
The documents the client has open, keyed by URI.

What is here is what the client is showing, which is not what is on disk. The
server answers from these and reads the disk only for files nobody has open.
"""

	_documents = Dictionary[of string, TextDocument]()

	Count as int:
		get: return _documents.Count

	Uris as IEnumerable[of string]:
		get: return _documents.Keys

	def Open(uri as string, languageId as string, version as int, text as string) as TextDocument:
		document = TextDocument(uri, languageId, version, text)
		_documents[uri] = document
		return document

	def Change(uri as string, version as int, text as string) as TextDocument:
	"""Returns null for a document the client never opened."""
		document = Get(uri)
		return null if document is null
		document.Update(version, text)
		return document

	def Close(uri as string):
		_documents.Remove(uri)

	def Get(uri as string) as TextDocument:
		document as TextDocument
		return document if _documents.TryGetValue(uri, document)
		return null
