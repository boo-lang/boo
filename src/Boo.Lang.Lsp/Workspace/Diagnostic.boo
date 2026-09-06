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
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

class Diagnostic:
"""
Turns what the compiler reports into what the client draws.

A CompilerError says where it starts and nothing about where it ends, so the
end of the range is the end of the word the client would see under the squiggle.
"""

	public static final Source = "boo"
	public static final Error = 1
	public static final Warning = 2

	# A client fades what it is told is unnecessary rather than drawing it.
	public static final Unnecessary = 1

	# What the compiler reports about something nothing uses.
	static final NeverUsed = ("BCW0014", "BCW0016")

	static def FromError(document as TextDocument, error as CompilerError):
		return Build(document, error.LexicalInfo, Error, error.Code, error.Message)

	static def FromWarning(document as TextDocument, warning as CompilerWarning):
		return Build(document, warning.LexicalInfo, Warning, warning.Code, warning.Message)

	static def Range(start as Position, finish as Position):
		# Not named range: that name is an overloaded builtin method, so
		# assigning to it and indexing it does not reach a local.
		span = Dictionary[of string, object]()
		span["start"] = Place(start)
		span["end"] = Place(finish)
		return span

	private static def Place(position as Position):
		place = Dictionary[of string, object]()
		place["line"] = position.Line
		place["character"] = position.Character
		return place

	private static def Build(document as TextDocument, location as LexicalInfo, severity as int, code as string, message as string):
		start = Positions.FromLexicalInfo(document, location)
		diagnostic = Dictionary[of string, object]()
		diagnostic["range"] = Range(start, EndOfWord(document, location, start, message))
		diagnostic["severity"] = severity
		diagnostic["code"] = code
		diagnostic["source"] = Source
		diagnostic["message"] = message
		if code in NeverUsed:
			tags = List[of object]()
			tags.Add(Unnecessary)
			diagnostic["tags"] = tags
		return diagnostic

	private static def EndOfWord(document as TextDocument, location as LexicalInfo, start as Position, message as string) as Position:
		# A location the compiler never set points at nothing to underline.
		return start unless location.Line > 0 and location.Column > 0

		line = document.LineText(start.Line)
		finish = start.Character
		while finish < line.Length and IsWordCharacter(line[finish]):
			finish++
		finish++ if finish == start.Character and finish < line.Length
		return Position(start.Line, Qualified(line, start.Character, finish, message))

	private static def Qualified(line as string, start as int, finish as int, message as string) as int:
	"""
	The end of the dotted name the message blames, or the end of the word.

	A namespace or an assembly is reported at its first segment, so the
	word alone underlines a fraction of what was not found. Only segments
	the message itself names are taken: in Application.Run the call is no
	part of an unknown Application.
	"""
		return finish if string.IsNullOrEmpty(message)
		taken = finish
		at = finish
		while at < line.Length and line[at] == char('.'):
			ahead = at + 1
			while ahead < line.Length and IsWordCharacter(line[ahead]):
				ahead++
			break if ahead == at + 1
			break unless message.Contains(line.Substring(start, ahead - start))
			taken = ahead
			at = ahead
		return taken

	private static def IsWordCharacter(c as char) as bool:
		return char.IsLetterOrDigit(c) or c == char('_')
