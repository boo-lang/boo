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
import System.Text

class BracketRepair:
"""
Balances the brackets in a document so it can be parsed for an outline.

A half typed bracket is what the buffer looks like while someone is working,
and the parser is unforgiving about it: an opener that never closes hides
every newline below it, so the block structure of the rest of the file is
lost, and an unmatched closer derails the parse just as badly.

Repair is a guess, and it is only ever fed to the parser. Diagnostics come
from the real text, so nothing the user is told is invented here.

Line count is preserved: a closer is added at the end of the line that opened
it, and an unmatched closer becomes a space. Positions taken from a parse of
the repaired text therefore still describe the real document.
"""

	static def Repair(text as string) as string:
		return text if text is null or text.Length == 0

		lines = Scan(text)
		return text unless lines.Repaired
		return lines.Text

	private static def Scan(text as string) as Repairing:
		result = Repairing(text)
		openers = List[of Opening]()
		i = 0
		line = 0

		while i < text.Length:
			c = text[i]

			if c == char('\n'):
				line++
				i++
				continue

			skipped = SkipPast(text, i, result)
			if skipped > i:
				i = skipped
				continue

			if c == char('(') or c == char('[') or c == char('{'):
				openers.Add(Opening(c, line))
			elif c == char(')') or c == char(']') or c == char('}'):
				if openers.Count == 0:
					# Nothing opened this, so the parser would only trip on it.
					result.Blank(i)
				else:
					openers.RemoveAt(openers.Count - 1)
			i++

		# Whatever is still open is closed at the end of the line that opened
		# it, innermost first.
		for j in range(openers.Count - 1, -1, -1):
			result.CloseAtEndOfLine(openers[j].Line, Closer(openers[j].Bracket))

		return result

	private static def SkipPast(text as string, i as int, result as Repairing) as int:
	"""Steps over a comment or a string, so their brackets are not counted."""
		c = text[i]

		if c == char('#'):
			return EndOfLine(text, i)
		if c == char('/') and Peek(text, i + 1) == char('/'):
			return EndOfLine(text, i)
		if c == char('/') and Peek(text, i + 1) == char('*'):
			return EndOfBlockComment(text, i)

		return i unless c == char('"') or c == char('\'')

		quote = QuoteAt(text, i)
		finish = EndOfString(text, i, quote)
		if finish < 0:
			# An unterminated string would hide the rest of the file, so it is
			# closed where it stops.
			result.CloseAtEndOfLine(LineOf(text, i), quote)
			return EndOfLine(text, i)
		return finish

	private static def EndOfBlockComment(text as string, i as int) as int:
	"""Block comments nest, so the first close does not always end one."""
		depth = 0
		at = i
		while at < text.Length - 1:
			if text[at] == char('/') and text[at + 1] == char('*'):
				depth++
				at += 2
				continue
			if text[at] == char('*') and text[at + 1] == char('/'):
				depth--
				at += 2
				return at if depth == 0
				continue
			at++
		return text.Length

	private static def QuoteAt(text as string, i as int) as string:
		c = text[i].ToString()
		return c + c + c if Peek(text, i + 1) == text[i] and Peek(text, i + 2) == text[i]
		return c

	private static def EndOfString(text as string, i as int, quote as string) as int:
		at = i + quote.Length
		while at < text.Length:
			if text[at] == char('\\'):
				at += 2
				continue
			return at + quote.Length if Matches(text, at, quote)
			# A single quoted string does not survive a line break.
			return -1 if text[at] == char('\n') and quote.Length == 1
			at++
		return -1

	private static def Matches(text as string, at as int, quote as string) as bool:
		return false if at + quote.Length > text.Length
		for k in range(quote.Length):
			return false unless text[at + k] == quote[k]
		return true

	private static def Peek(text as string, i as int) as char:
		return char(0) if i >= text.Length
		return text[i]

	private static def EndOfLine(text as string, i as int) as int:
		at = text.IndexOf(char('\n'), i)
		return text.Length if at < 0
		return at

	private static def LineOf(text as string, i as int) as int:
		line = 0
		for k in range(i):
			line++ if text[k] == char('\n')
		return line

	private static def Closer(opener as char) as string:
		return ")" if opener == char('(')
		return "]" if opener == char('[')
		return "}"

	private class Opening:
		public final Bracket as char
		public final Line as int

		def constructor(bracket as char, line as int):
			Bracket = bracket
			Line = line

	private class Repairing:
	"""Collects the edits, then applies them in one pass."""

		_text as string
		_blanked = List[of int]()
		_appended = Dictionary[of int, string]()

		def constructor(text as string):
			_text = text

		Repaired as bool:
			get: return _blanked.Count > 0 or _appended.Count > 0

		def Blank(at as int):
			_blanked.Add(at)

		def CloseAtEndOfLine(line as int, closer as string):
			existing as string
			_appended.TryGetValue(line, existing)
			# Innermost closes first, and openers arrive innermost first.
			_appended[line] = existing + closer

		Text as string:
			get:
				builder = StringBuilder()
				line = 0
				for i in range(_text.Length):
					if _text[i] == char('\n'):
						builder.Append(TakeAppended(line))
						line++
						builder.Append(_text[i])
						continue
					builder.Append(' ') if _blanked.Contains(i)
					builder.Append(_text[i]) unless _blanked.Contains(i)
				builder.Append(TakeAppended(line))
				return builder.ToString()

		private def TakeAppended(line as int) as string:
			closers as string
			return "" unless _appended.TryGetValue(line, closers)
			return closers
