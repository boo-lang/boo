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

import System
import System.Collections.Generic

class TextDocument:
"""
One open document, with an index of where its lines start.

The index is rebuilt whenever the text changes, which keeps a position lookup
to a binary search rather than a scan of the whole buffer.
"""

	public final Uri as string
	public final LanguageId as string

	[property(Version)]
	_version as int

	[property(Text)]
	_text as string

	_lineStarts as List[of int]

	def constructor(uri as string, languageId as string, version as int, text as string):
		Uri = uri
		LanguageId = languageId
		Update(version, text)

	def Update(version as int, text as string):
		_version = version
		_text = text
		_lineStarts = IndexLines(text)

	LineCount as int:
		get: return _lineStarts.Count

	def LineText(line as int) as string:
		return "" if line < 0 or line >= _lineStarts.Count
		start = _lineStarts[line]
		finish = EndOfLine(line)
		return _text.Substring(start, finish - start)

	def OffsetAt(position as Position) as int:
		return 0 if position.Line < 0
		return _text.Length if position.Line >= _lineStarts.Count
		start = _lineStarts[position.Line]
		return start if position.Character < 0
		return Math.Min(start + position.Character, EndOfLine(position.Line))

	def PositionAt(offset as int) as Position:
		return Position(0, 0) if offset <= 0
		clamped = Math.Min(offset, _text.Length)
		line = LineAt(clamped)
		return Position(line, clamped - _lineStarts[line])

	private def EndOfLine(line as int) as int:
	"""The offset of the line terminator, or of the end of the text."""
		finish = _text.Length
		if line + 1 < _lineStarts.Count:
			finish = _lineStarts[line + 1] - 1
			finish-- if finish > 0 and _text[finish - 1] == char('\r')
		return finish

	private def LineAt(offset as int) as int:
		low = 0
		high = _lineStarts.Count - 1
		while low < high:
			middle = (low + high + 1) / 2
			if _lineStarts[middle] <= offset:
				low = middle
			else:
				high = middle - 1
		return low

	private static def IndexLines(text as string) as List[of int]:
		starts = List[of int]()
		starts.Add(0)
		for i in range(text.Length):
			starts.Add(i + 1) if text[i] == char('\n')
		return starts
