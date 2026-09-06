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
import System.Collections.Generic
import Boo.Lang.Lsp.Json

callable RequestHandler(params as object) as object

callable NotificationHandler(params as object)

callable RequestGuard(method as string) as Dictionary[of string, object]

class Connection:
"""
Reads messages off a MessageStream and hands them to the registered handlers.

Messages are read on their own thread and answered one at a time, so a
cancellation is noted while the request it names is still waiting its turn.
Reading in step with answering would take the request first every time, since
a client sends the cancellation second.
"""

	public static final CancelRequest = "$/cancelRequest"

	_stream as MessageStream
	_requests = Dictionary[of string, RequestHandler]()
	_notifications = Dictionary[of string, NotificationHandler]()
	_cancelled = HashSet[of string]()
	_incoming = Queue[of string]()
	_listening = false
	_guard as RequestGuard

	def constructor(stream as MessageStream):
		_stream = stream

	def OnRequest(method as string, handler as RequestHandler):
		_requests[method] = handler

	def OnNotification(method as string, handler as NotificationHandler):
		_notifications[method] = handler

	def Guard(guard as RequestGuard):
	"""Vets every request before its handler runs; a body means refuse."""
		_guard = guard

	def Notify(method as string, params as object):
	"""Sends a notification to the client. Nothing answers it."""
		Reply(JsonRpc.Notification(method, params))

	def Listen():
	"""Answers messages until the client closes the stream or Stop is called."""
		_listening = true
		reader = Thread(ReadAhead)
		reader.IsBackground = true
		reader.Start()
		while _listening:
			message = Take()
			break if message is null
			Handle(message)

	def Stop():
		_listening = false
		Put(null)

	private def ReadAhead():
	"""
	Reads messages while a handler is running.

	A client sends a cancellation after the request it names, so reading
	only between handlers would always take the request first and the word
	to drop it second.
	"""
		while true:
			message = _stream.Read()
			if message is null:
				Put(null)
				return
			continue if TakenAsCancel(message)
			Put(message)

	private def TakenAsCancel(message as string) as bool:
	"""Whether this was a cancellation, which is answered by noting it."""
		try:
			parsed = JsonCodec.Parse(message) as Dictionary[of string, object]
			return false if parsed is null
			return false unless parsed.ContainsKey("method")
			return false unless parsed["method"] as string == CancelRequest
			params as object
			parsed.TryGetValue("params", params)
			Cancel(params)
			return true
		except:
			# Whatever it is, let the handler report it in turn.
			return false

	private def Put(message as string):
		lock _incoming:
			_incoming.Enqueue(message)
			Monitor.Pulse(_incoming)

	private def Take() as string:
		lock _incoming:
			Monitor.Wait(_incoming) while _incoming.Count == 0
			return _incoming.Dequeue()

	private def Handle(message as string):
		parsed as Dictionary[of string, object]
		try:
			parsed = JsonCodec.Parse(message) as Dictionary[of string, object]
		except e as Exception:
			Reply(JsonRpc.Error(null, JsonRpc.ParseError, e.Message))
			return

		if parsed is null:
			Reply(JsonRpc.Error(null, JsonRpc.ParseError, "message is not a JSON object"))
			return

		id as object
		hasId = parsed.TryGetValue("id", id)
		method = parsed["method"] as string if parsed.ContainsKey("method")

		if method is null:
			# A reply to something the server asked for. Nothing wants it yet.
			return if parsed.ContainsKey("result") or parsed.ContainsKey("error")
			replyTo as object
			replyTo = id if hasId
			Reply(JsonRpc.Error(replyTo, JsonRpc.InvalidRequest, "message has no method"))
			return

		params as object
		parsed.TryGetValue("params", params)

		if hasId:
			HandleRequest(id, method, params)
		else:
			HandleNotification(method, params)

	private def HandleRequest(id as object, method as string, params as object):
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
		map = params as Dictionary[of string, object]
		return if map is null
		id as object
		return unless map.TryGetValue("id", id)
		# Noted by the reading thread, read by the answering one.
		lock _cancelled:
			_cancelled.Add(KeyOf(id))

	private def Cancelled(id as object) as bool:
		lock _cancelled:
			return _cancelled.Remove(KeyOf(id))

	private static def KeyOf(id as object) as string:
		return "" if id is null
		return id.ToString()

	private def Reply(message as Dictionary[of string, object]):
		_stream.Write(JsonCodec.Stringify(message))

	private def Log(message as string):
		# stdout carries the protocol, so anything else has to go to stderr.
		Console.Error.WriteLine("boo-ls: ${message}")
