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

namespace Boo.Lang.Lsp.Protocol

import System.Collections.Generic

class JsonRpc:
"""The JSON-RPC vocabulary the protocol defines, and the shapes of a reply."""

	public static final Version = "2.0"

	public static final ParseError = -32700
	public static final InvalidRequest = -32600
	public static final MethodNotFound = -32601
	public static final InvalidParams = -32602
	public static final InternalError = -32603

	# LSP adds these to the codes JSON-RPC reserves.
	public static final ServerNotInitialized = -32002
	public static final RequestCancelled = -32800
	public static final ContentModified = -32801

	static def Result(id as object, result as object):
		message = Envelope(id)
		message["result"] = result
		return message

	static def Error(id as object, code as int, description as string):
		return ErrorReply(id, ErrorBody(code, description))

	static def Notification(method as string, params as object):
		message = Dictionary[of string, object]()
		message["jsonrpc"] = Version
		message["method"] = method
		message["params"] = params
		return message

	static def ErrorBody(code as int, description as string):
		error = Dictionary[of string, object]()
		error["code"] = code
		error["message"] = description
		return error

	static def ErrorReply(id as object, body as Dictionary[of string, object]):
		message = Envelope(id)
		message["error"] = body
		return message

	private static def Envelope(id as object):
		message = Dictionary[of string, object]()
		message["jsonrpc"] = Version
		message["id"] = id
		return message
