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

class CallSignature:
"""
The call being written at the cursor, and which argument it is on.

Text under a cursor is half typed by definition: an open bracket with nothing
closing it. Repairing the brackets is what makes it bind at all, and since the
repair keeps every position where it was, what comes back still describes the
real document.
"""

	class Result:
		public Overloads as List[of Signatures.Overload]
		public ActiveOverload as int
		public ActiveParameter as int

	_analyzer = Analyzer()

	def At(document as TextDocument, position as Position) as Result:
		call = Call.At(document, position)
		return null if call is null

		# Closing the call where the cursor is, rather than balancing the whole
		# document: an opener here would otherwise pair with a closer further
		# down the file, and the statement never closes at all. Everything
		# before the cursor keeps its position, so the callee is still where
		# it was.
		at = document.OffsetAt(position)
		closed = document.Text.Insert(at, Filling(document.Text, at) + call.Closers)
		repaired = TextDocument(document.Uri, document.LanguageId, document.Version, closed)
		found = Lookup.At(repaired, _analyzer.Bound(repaired), call.Callee)
		return null if found is null or found.Overloads.Count == 0

		return Result(
			Overloads: found.Overloads,
			ActiveOverload: Fitting(found.Overloads, call.Argument),
			ActiveParameter: call.Argument)

	private static def Filling(text as string, at as int) as string:
	"""
	An argument to stand in for the one not written yet, where one is needed.

	A call left open after a comma has an argument missing, and closing it
	on the spot gives a trailing comma the parser will not take. Anywhere
	else the brackets alone are enough.
	"""
		i = at - 1
		i-- while i >= 0 and (text[i] == char(' ') or text[i] == char('\t'))
		return "null" if i >= 0 and text[i] == char(',')
		return ""

	private static def Fitting(overloads as List[of Signatures.Overload], argument as int) as int:
	"""
	The shortest overload the arguments written so far still fit, or the first.

	Which one is meant is not known until the call is finished, so the guess
	is the narrowest one that still has a parameter for what is being typed:
	a four argument overload is a poor thing to show someone typing the
	first of two.
	"""
		best = -1
		for i in range(overloads.Count):
			continue if overloads[i].Parameters.Count <= argument
			best = i if best < 0 or overloads[i].Parameters.Count < overloads[best].Parameters.Count
		return 0 if best < 0
		return best

	private class Call:
	"""Where the name being called is, and how many commas precede the cursor."""

		public final Callee as Position
		public final Argument as int
		public final Closers as string

		# A call spanning more lines than this is not worth walking back over.
		static final Reach = 40

		def constructor(callee as Position, argument as int, closers as string):
			Callee = callee
			Argument = argument
			Closers = closers

		static def At(document as TextDocument, position as Position) as Call:
			return null if position.Line < 0
			cursor = document.OffsetAt(position)

			# Scanning starts at the cursor's own line, which is where a call
			# usually opens. A call opened further up is found by starting a
			# line earlier and reading forward again.
			line = position.Line
			floor = 0
			floor = position.Line - Reach if position.Line > Reach
			while line >= floor:
				found = Scan(document, line, cursor)
				return found if found is not null
				line--
			return null

		private static def Scan(document as TextDocument, fromLine as int, cursor as int) as Call:
			text = document.Text
			opens = List[of int]()
			commas = List[of int]()

			i = document.OffsetAt(Position(fromLine, 0))
			while i < cursor:
				c = text[i]
				if c == char('"') or c == char('\''):
					i = EndOfString(text, i, cursor)
					continue
				if c == char('#'):
					i = EndOfLine(text, i, cursor)
					continue
				if c == char('(') or c == char('[') or c == char('{'):
					opens.Add(i)
					commas.Add(0)
				elif c == char(')') or c == char(']') or c == char('}'):
					unless opens.Count == 0:
						opens.RemoveAt(opens.Count - 1)
						commas.RemoveAt(commas.Count - 1)
				elif c == char(',') and opens.Count > 0:
					commas[commas.Count - 1] = commas[commas.Count - 1] + 1
				i++

			return null if opens.Count == 0
			open = opens[opens.Count - 1]
			# A list or a hash is open, not a call.
			return null if text[open] != char('(')

			callee = NameBefore(document, text, open)
			return null if callee is null
			return Call(callee, commas[commas.Count - 1], Closing(text, opens))

		private static def Closing(text as string, opens as List[of int]) as string:
		"""What it takes to close everything still open, innermost first."""
			closers = ""
			for i in range(opens.Count - 1, -1, -1):
				opener = text[opens[i]]
				closers += ")" if opener == char('(')
				closers += "]" if opener == char('[')
				closers += "}" if opener == char('{')
			return closers

		private static def NameBefore(document as TextDocument, text as string, open as int) as Position:
		"""Where the name in front of the bracket starts, or null if there is none."""
			last = open - 1
			last-- while last >= 0 and (text[last] == char(' ') or text[last] == char('\t'))
			return null if last < 0 or not IsWord(text[last])

			first = last
			first-- while first > 0 and IsWord(text[first - 1])
			return document.PositionAt(first)

		private static def EndOfString(text as string, at as int, limit as int) as int:
		"""Just past the string that starts here, or the limit if it never ends."""
			quote = text[at]
			triple = at + 2 < limit and text[at + 1] == quote and text[at + 2] == quote
			i = at + (3 if triple else 1)
			while i < limit:
				return i + 1 if not triple and text[i] == quote
				return i + 3 if triple and i + 2 < limit and text[i] == quote and text[i + 1] == quote and text[i + 2] == quote
				i++ if text[i] == char('\\')
				# A single quoted string ends at the line it started on.
				return i if not triple and text[i] == char('\n')
				i++
			return limit

		private static def EndOfLine(text as string, at as int, limit as int) as int:
			i = at
			i++ while i < limit and text[i] != char('\n')
			return i

		private static def IsWord(c as char) as bool:
			return char.IsLetterOrDigit(c) or c == char('_')
