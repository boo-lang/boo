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
import Boo.Lang.Lsp.Protocol
import Boo.Lang.Lsp.Workspace

class DiagnosticsPublisher:
"""
Tells the client what is wrong with a document, and what no longer is.

Every publish carries the whole list for that document, so an empty list is
how a fixed file gets its squiggles taken away. A closed document is cleared
for the same reason.
"""

	public static final Method = "textDocument/publishDiagnostics"

	_connection as Connection
	_analyzer = Analyzer()

	def constructor(connection as Connection):
		_connection = connection

	def PublishSyntax(document as TextDocument):
	"""
	The fast tier, run on the message loop for every change.

	It publishes only when parsing found something. A clean parse says nothing,
	because saying nothing keeps the semantic diagnostics already on screen
	from flickering off and back on between keystrokes.
	"""
		diagnostics = _analyzer.Parse(document)
		Send(document.Uri, diagnostics) if diagnostics.Count > 0

	def PublishSemantic(document as TextDocument):
	"""The slow tier, run on the worker. Its answer is the whole truth."""
		Send(document.Uri, _analyzer.Bind(document))

	def Clear(uri as string):
		Send(uri, List[of object]())

	private def Send(uri as string, diagnostics as List[of object]):
		params = Dictionary[of string, object]()
		params["uri"] = uri
		params["diagnostics"] = diagnostics
		_connection.Notify(Method, params)
