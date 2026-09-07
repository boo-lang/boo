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

import System
import System.Threading.Tasks
import Boo.Lang.Lsp.Json
import Boo.Lang.Lsp.Protocol
import Boo.Lang.Lsp.Workspace
import System.Text.Json.Nodes

class LanguageServer:
"""
A session: the lifecycle handshake, and the handlers registered on top of it.

The protocol lets a client send only initialize until the server has answered
it, and only exit once shutdown has been answered. Both rules are enforced
here so that no feature handler has to think about them.
"""

	_connection as Connection
	_documents = DocumentStore()
	_sync as TextDocumentSync
	_diagnostics as DiagnosticsPublisher
	_worker as AnalysisWorker
	_initialized = false
	_shuttingDown = false
	_exited = false

	public static final DebounceMilliseconds = 300

	def constructor(stream as MessageStream):
		self(Connection(stream), DebounceMilliseconds)

	def constructor(stream as MessageStream, debounceMilliseconds as int):
		self(Connection(stream), debounceMilliseconds)

	def constructor(connection as Connection):
		self(connection, DebounceMilliseconds)

	def constructor(connection as Connection, debounceMilliseconds as int):
		_connection = connection
		_connection.OnRequest("initialize", Initialize)
		_connection.OnRequest("shutdown", Shutdown)
		_connection.OnNotification("initialized", Initialized)
		_connection.OnNotification("exit", Exit)
		_connection.Guard(CheckLifecycle)
		_sync = TextDocumentSync(_documents, _connection)
		_diagnostics = DiagnosticsPublisher(_connection)
		_worker = AnalysisWorker(_diagnostics.PublishSemantic, debounceMilliseconds)
		_sync.Changed = Changed
		_sync.Closed = Closed

	Documents as DocumentStore:
		get: return _documents

	[async] def RunAsync() as Task[of int]:
	"""
	Serves one session and returns the exit code the process should use.
	"""
		_worker.Start()
		try:
			await(_connection.ListenAsync())
		ensure:
			# Let whatever is mid-compile finish and publish before going.
			_worker.WaitForIdle(DebounceMilliseconds * 10)
			await(_worker.StopAsync())
			await(_connection.DrainAsync())
		# A client that closes the pipe without shutting down is an error too.
		return 0 if _shuttingDown
		return 1

	private def Changed(document as TextDocument):
		_diagnostics.PublishSyntax(document)
		_worker.Submit(document)

	private def Closed(uri as string):
		_worker.Withdraw(uri)
		_diagnostics.Clear(uri)

	private def CheckLifecycle(method as string) as JsonObject:
		if method == "initialize":
			return JsonRpc.ErrorBody(JsonRpc.InvalidRequest, "the server is already initialized") if _initialized
			return null
		return JsonRpc.ErrorBody(JsonRpc.InvalidRequest, "the server has shut down") if _shuttingDown
		return JsonRpc.ErrorBody(JsonRpc.ServerNotInitialized, "the server has not been initialized") unless _initialized
		return null

	private def Initialize(params as object) as JsonNode:
		_initialized = true
		return Json({
			"capabilities": Capabilities(),
			"serverInfo": { "name": ServerInfo.Name, "version": ServerInfo.Version }
		})

	private def Capabilities():
		return Json({
			"textDocumentSync": TextDocumentSync.Capability()
		})

	private def Initialized(params as object):
		pass

	private def Shutdown(params as object) as JsonNode:
		_shuttingDown = true
		return null

	private def Exit(params as object):
		_exited = true
		_connection.Stop()
