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

namespace Boo.Lang.Lsp.Server

import System.Collections.Generic
import Boo.Lang.Lsp.Json
import Boo.Lang.Lsp.Protocol
import Boo.Lang.Lsp.Workspace

class Navigation:
"""
Answers hover and go to definition, both from the same lookup.

Each request binds the document afresh, which is affordable for one file and
is what the cached context in M8 is meant to replace.
"""

	public static final Hover = "textDocument/hover"
	public static final Definition = "textDocument/definition"

	_documents as DocumentStore
	_analyzer = Analyzer()

	def constructor(documents as DocumentStore, connection as Connection):
		_documents = documents
		connection.OnRequest(Hover, Describe)
		connection.OnRequest(Definition, Locate)

	private def Describe(params as object) as object:
		found = At(params)
		return null if found is null

		# The signature reads as Boo, what it documents reads as prose, and
		# the blank line between them is what keeps markdown from running the
		# two together.
		value = "```boo\n${found.Signature}\n```"
		value += "\n\n" + found.Documentation unless string.IsNullOrEmpty(found.Documentation)

		contents = Dictionary[of string, object]()
		contents["kind"] = "markdown"
		contents["value"] = value

		hover = Dictionary[of string, object]()
		hover["contents"] = contents
		hover["range"] = Diagnostic.Range(found.Start, found.End)
		return hover

	private def Locate(params as object) as object:
		found = At(params)
		return null if found is null or not found.HasDeclaration

		# A name is what is jumped to, and its length is not recorded, so the
		# range is empty and the editor lands on the first character.
		location = Dictionary[of string, object]()
		location["uri"] = found.DeclarationUri
		location["range"] = Diagnostic.Range(found.Declaration, found.Declaration)
		return location

	private def At(params as object) as Lookup.Result:
		document = _documents.Get(Fields.Text(Fields.Map(params, "textDocument"), "uri"))
		return null if document is null

		position = Fields.Map(params, "position")
		return null if position is null

		return Lookup.At(
			document,
			_analyzer.Bound(document),
			Position(Fields.Number(position, "line", 0), Fields.Number(position, "character", 0)))
