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
import System.Threading.Tasks

class MessageStream:
"""
Carries LSP base protocol messages over a pair of byte streams.

Bytes rather than a TextReader, which would decode across the header boundary
and could swallow half of a UTF-8 sequence at the end of its buffer.

One task reads and one task writes, so neither end is locked.
"""

	static final ContentLength = "content-length"
	static final BufferSize = 4096

	_input as Stream
	_output as Stream
	_encoding = UTF8Encoding(false)
	_buffer = array(byte, BufferSize)
	_start as int
	_end as int

	def constructor(input as Stream, output as Stream):
		_input = input
		_output = output

	[async] def ReadAsync() as Task[of string]:
	"""Returns the next message, or null once the stream has ended."""
		length = await(ReadHeadersAsync())
		return null if length < 0
		body = await(ReadBodyAsync(length))
		return _encoding.GetString(body)

	[async] def WriteAsync(message as string) as Task:
		body = _encoding.GetBytes(message)
		header = _encoding.GetBytes("Content-Length: ${body.Length}\r\n\r\n")
		await(_output.WriteAsync(header, 0, header.Length))
		await(_output.WriteAsync(body, 0, body.Length))
		await(_output.FlushAsync())

	[async] private def ReadHeadersAsync() as Task[of int]:
		length = -1
		while true:
			line = await(ReadHeaderLineAsync())
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

	[async] private def ReadHeaderLineAsync() as Task[of string]:
		line = StringBuilder()
		while true:
			b = await(NextByteAsync())
			if b < 0:
				# A header cut off midway is a broken stream, not a clean end.
				raise ProtocolError("stream ended inside a header") if line.Length > 0
				return null
			continue if b == 13
			return line.ToString() if b == 10
			line.Append(cast(char, b))

	[async] private def NextByteAsync() as Task[of int]:
		if _start == _end:
			filled = await(RefillAsync())
			return -1 unless filled
		b as int = _buffer[_start]
		_start++
		return b

	[async] private def RefillAsync() as Task[of bool]:
	"""Reads the next chunk, false once the stream has ended."""
		_start = 0
		_end = await(_input.ReadAsync(_buffer, 0, BufferSize))
		return _end > 0

	[async] private def ReadBodyAsync(length as int) as Task[of (byte)]:
		body = array(byte, length)
		read = 0
		while read < length:
			if _start == _end:
				filled = await(RefillAsync())
				raise ProtocolError("stream ended after ${read} of ${length} bytes") unless filled
			got = Math.Min(_end - _start, length - read)
			Array.Copy(_buffer, _start, body, read, got)
			_start += got
			read += got
		return body
