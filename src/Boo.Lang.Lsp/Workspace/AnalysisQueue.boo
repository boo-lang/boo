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

class AnalysisQueue:
"""
What is waiting to be compiled, one document per URI.

A keystroke makes the version before it worthless, so a second submission of
the same document replaces the first rather than queueing behind it. Order of
first submission is kept, so the file the user is typing in stays ahead of the
ones that merely reference it.
"""

	_pending = Dictionary[of string, TextDocument]()
	_order = List[of string]()
	_lock = object()

	Count as int:
		get:
			lock _lock:
				return _pending.Count

	def Submit(document as TextDocument):
		lock _lock:
			_order.Add(document.Uri) unless _pending.ContainsKey(document.Uri)
			_pending[document.Uri] = document

	def Withdraw(uri as string):
		lock _lock:
			_pending.Remove(uri)
			_order.Remove(uri)

	def Drain() as List[of TextDocument]:
		drained = List[of TextDocument]()
		lock _lock:
			for uri in _order:
				document as TextDocument
				drained.Add(document) if _pending.TryGetValue(uri, document)
			_pending.Clear()
			_order.Clear()
		return drained
