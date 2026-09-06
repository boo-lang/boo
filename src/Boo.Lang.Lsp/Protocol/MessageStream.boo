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
import System.IO
import System.Text

class MessageStream:
"""
Carries LSP base protocol messages over a pair of byte streams.

Headers are read a byte at a time and the body is read by count, because a
TextReader would decode across the header boundary and could swallow half of a
UTF-8 sequence at the end of its buffer.
"""

	static final ContentLength = "content-length"

	_input as Stream
	_output as Stream
	_encoding = UTF8Encoding(false)
	# The analysis worker publishes while the message loop replies, and two
	# half written messages on one pipe is not a protocol.
	_writing = object()

	def constructor(input as Stream, output as Stream):
		_input = input
		_output = output

	def Read() as string:
	"""Returns the next message, or null once the stream has ended."""
		length = ReadHeaders()
		return null if length < 0
		return _encoding.GetString(ReadBody(length))

	def Write(message as string):
		body = _encoding.GetBytes(message)
		header = _encoding.GetBytes("Content-Length: ${body.Length}\r\n\r\n")
		lock _writing:
			_output.Write(header, 0, header.Length)
			_output.Write(body, 0, body.Length)
			_output.Flush()

	private def ReadHeaders() as int:
		length = -1
		while true:
			line = ReadHeaderLine()
			return -1 if line is null
			if line.Length == 0:
				raise ProtocolError("header block has no Content-Length") if length < 0
				return length
			separator = line.IndexOf(char(':'))
			continue if separator < 0
			name = line.Substring(0, separator).Trim().ToLowerInvariant()
			continue unless name == ContentLength
			value = line.Substring(separator + 1).Trim()
			parsed as int
			raise ProtocolError("Content-Length is not a number: ${value}") unless int.TryParse(value, parsed)
			length = parsed

	private def ReadHeaderLine() as string:
		line = StringBuilder()
		while true:
			b = _input.ReadByte()
			if b < 0:
				# A header cut off midway is a broken stream, not a clean end.
				raise ProtocolError("stream ended inside a header") if line.Length > 0
				return null
			if b == 13:
				continue
			if b == 10:
				return line.ToString()
			line.Append(cast(char, b))

	private def ReadBody(length as int) as (byte):
		body = array(byte, length)
		read = 0
		while read < length:
			got = _input.Read(body, read, length - read)
			raise ProtocolError("stream ended after ${read} of ${length} bytes") if got <= 0
			read += got
		return body
