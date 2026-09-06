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

class SemanticTokenHandler:
"""Answers full-document semantic token requests."""

	public static final FullMethod = "textDocument/semanticTokens/full"

	_documents as DocumentStore
	_analyzer = Analyzer()

	def constructor(documents as DocumentStore, connection as Connection):
		_documents = documents
		connection.OnRequest(FullMethod, AnswerFull)

	static def Capability() as Dictionary[of string, object]:
		return Boo.Lang.Lsp.Workspace.SemanticTokens.Capability()

	private def AnswerFull(params as object) as object:
		document = _documents.Get(Fields.Text(Fields.Map(params, "textDocument"), "uri"))
		return Empty() if document is null
		return Boo.Lang.Lsp.Workspace.SemanticTokens.Of(document, _analyzer.Bound(document))

	private static def Empty() as Dictionary[of string, object]:
		result = Dictionary[of string, object]()
		result["data"] = List[of long]()
		return result
