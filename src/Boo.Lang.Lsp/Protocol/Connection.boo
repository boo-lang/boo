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

import System
import System.Threading
import System.Threading.Channels
import System.Threading.Tasks
import System.Collections.Generic
import Boo.Lang.Lsp.Json
import System.Text.Json.Nodes

callable RequestHandler(params as object) as JsonNode

callable NotificationHandler(params as object)

callable RequestGuard(method as string) as JsonObject

class Connection:
"""
Reads messages off a MessageStream and hands them to the registered handlers.

Reading, answering and writing each run on their own task. Reading runs ahead
so a cancellation, which a client sends after the request it names, is noted
while that request is still waiting its turn.
"""

	public static final CancelRequest = "$/cancelRequest"

	_stream as MessageStream
	_requests = Dictionary[of string, RequestHandler]()
	_notifications = Dictionary[of string, NotificationHandler]()
	_cancelled = HashSet[of string]()
	# Countable rather than single reader: what is queued says which
	# cancellations still matter.
	_incoming = Channel.CreateUnbounded[of string](
		UnboundedChannelOptions(SingleWriter: true))
	_outgoing = Channel.CreateUnbounded[of string](
		UnboundedChannelOptions(SingleReader: true))
	_guard as RequestGuard
	_writing as Task
	_listening = true

	def constructor(stream as MessageStream):
		_stream = stream

	def OnRequest(method as string, handler as RequestHandler):
		_requests[method] = handler

	def OnNotification(method as string, handler as NotificationHandler):
		_notifications[method] = handler

	def Guard(guard as RequestGuard):
	"""Vets every request before its handler runs; a body means refuse."""
		_guard = guard

	def Notify(method as string, params as JsonNode):
	"""Sends a notification to the client. Nothing answers it."""
		Reply(JsonRpc.Notification(method, params))

	[async] def ListenAsync() as Task:
	"""Answers messages until the client closes the stream or Stop is called."""
		_writing = Reported(WriteLoopAsync(), "writing")
		# Not awaited, since a read already blocked on stdin cannot be called back.
		# It completes the queue rather than stopping, so what was read is answered.
		Reported(ReadLoopAsync(), "reading").ContinueWith({ done as Task | _incoming.Writer.TryComplete() })
		while _listening:
			message = await(NextAsync(_incoming.Reader))
			break if message is null
			Handle(message)
			ForgetCancellations() if _incoming.Reader.Count == 0

	def Stop():
	"""Stops answering. What is already queued is dropped, as exit expects."""
		_listening = false
		_incoming.Writer.TryComplete()

	[async] def DrainAsync() as Task:
	"""
	Sends what is still queued and stops writing.

	Separate from Listen because the worker publishes its last diagnostics
	after the message loop has let go.
	"""
		_outgoing.Writer.TryComplete()
		return if _writing is null
		await(_writing)

	[async] private static def NextAsync(reader as ChannelReader[of string]) as Task[of string]:
	"""
	The next message, or null once the channel is finished and drained.
	"""
		message as string
		while true:
			return message if reader.TryRead(message)
			ready = await(reader.WaitToReadAsync().AsTask())
			return null unless ready

	[async] private def ReadLoopAsync() as Task:
		while true:
			message = await(_stream.ReadAsync())
			break if message is null
			continue if TakenAsCancel(message)
			break unless _incoming.Writer.TryWrite(message)

	[async] private def WriteLoopAsync() as Task:
		while true:
			message = await(NextAsync(_outgoing.Reader))
			break if message is null
			await(_stream.WriteAsync(message))

	[async] private def Reported(loop as Task, what as string) as Task:
	"""Notes a loop that fell over."""
		try:
			await(loop)
		except e as Exception:
			Log("${what} failed: ${e.Message}")

	private def TakenAsCancel(message as string) as bool:
	"""Whether this was a cancellation, which is answered by noting it."""
		try:
			parsed = JsonCodec.Parse(message) as JsonObject
			return false if parsed is null
			return false unless Fields.Text(parsed, "method") == CancelRequest
			Cancel(Fields.Of(parsed, "params"))
			return true
		except:
			# Whatever it is, let the handler report it in turn.
			return false

	private def Handle(message as string):
		parsed as JsonObject
		try:
			parsed = JsonCodec.Parse(message) as JsonObject
		except e as Exception:
			Reply(JsonRpc.Error(null, JsonRpc.ParseError, e.Message))
			return

		if parsed is null:
			Reply(JsonRpc.Error(null, JsonRpc.ParseError, "message is not a JSON object"))
			return

		id = Fields.Of(parsed, "id")
		method = Fields.Text(parsed, "method")

		if method is null:
			# A reply to something the server asked for. Nothing wants it yet.
			return if parsed.ContainsKey("result") or parsed.ContainsKey("error")
			Reply(JsonRpc.Error(id, JsonRpc.InvalidRequest, "message has no method"))
			return

		params = Fields.Of(parsed, "params")

		if id is not null:
			HandleRequest(id, method, params)
		else:
			HandleNotification(method, params)

	private def HandleRequest(id as JsonNode, method as string, params as object):
		if Cancelled(id):
			Reply(JsonRpc.Error(id, JsonRpc.RequestCancelled, "request ${KeyOf(id)} was cancelled"))
			return

		if _guard is not null:
			refusal = _guard(method)
			if refusal is not null:
				Reply(JsonRpc.ErrorReply(id, refusal))
				return

		handler as RequestHandler
		unless _requests.TryGetValue(method, handler):
			Reply(JsonRpc.Error(id, JsonRpc.MethodNotFound, "no handler for ${method}"))
			return

		try:
			Reply(JsonRpc.Result(id, handler(params)))
		except as OperationCanceledException:
			Reply(JsonRpc.Error(id, JsonRpc.RequestCancelled, "request ${KeyOf(id)} was cancelled"))
		except e as Exception:
			Reply(JsonRpc.Error(id, JsonRpc.InternalError, e.Message))

	private def HandleNotification(method as string, params as object):
		if method == CancelRequest:
			Cancel(params)
			return

		handler as NotificationHandler
		# An unknown notification is dropped, as the protocol requires.
		return unless _notifications.TryGetValue(method, handler)
		try:
			handler(params)
		except e as Exception:
			Log("${method} failed: ${e.Message}")

	private def Cancel(params as object):
		map = params as JsonObject
		return if map is null
		id as JsonNode
		return unless map.TryGetPropertyValue("id", id)
		# Noted by the reading task, read by the answering one.
		lock _cancelled:
			_cancelled.Add(KeyOf(id))

	private def Cancelled(id as object) as bool:
		lock _cancelled:
			return _cancelled.Remove(KeyOf(id))

	private def ForgetCancellations():
	"""
	Drops what the client cancelled once nothing is queued.

	A cancellation names a request already sent, so an empty queue means every
	one still held has been answered.
	"""
		lock _cancelled:
			_cancelled.Clear()

	private static def KeyOf(id as JsonNode) as string:
		return "" if id is null
		return id.ToString()

	private def Reply(message as JsonObject):
		_outgoing.Writer.TryWrite(JsonCodec.Stringify(message))

	private def Log(message as string):
		# stdout carries the protocol, so anything else has to go to stderr.
		Console.Error.WriteLine("boo-ls: ${message}")
