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
import Boo.Lang.Compiler.Ast
import Boo.Lang.Parser

class Positions:
"""
The one place LSP numbering and compiler numbering meet.

LSP counts lines and characters from zero. The compiler counts lines and
columns from one, and uses values below one for a location it never set.

They also disagree about tabs. LSP counts a tab as one character; the lexer
advances it to the next tab stop, so on an indented line, which in Boo is most
of them, the column is not the character. Crossing over means replaying that
expansion against the text of the line.
"""

	public static final TabSize = ParserSettings.DefaultTabSize

	static def FromSourceLocation(location as SourceLocation) as Position:
		return Position(Below(location.Line), Below(location.Column))

	static def ToLexicalInfo(fileName as string, position as Position) as LexicalInfo:
		return LexicalInfo(fileName, position.Line + 1, position.Character + 1)

	static def FromLexicalInfo(document as TextDocument, location as SourceLocation) as Position:
		line = Below(location.Line)
		return Position(line, CharacterAt(document.LineText(line), location.Column))

	static def ToLexicalInfo(document as TextDocument, position as Position) as LexicalInfo:
		column = ColumnAt(document.LineText(position.Line), position.Character)
		return LexicalInfo(document.Uri, position.Line + 1, column)

	static def CharacterAt(line as string, column as int) as int:
	"""The character index the lexer would have called this column."""
		return 0 if column < 1
		at = 1
		for i in range(line.Length):
			return i if at >= column
			at = Advance(at, line[i])
		return line.Length

	static def ColumnAt(line as string, character as int) as int:
	"""The column the lexer would report for this character index."""
		return 1 if character < 1
		at = 1
		for i in range(Math.Min(character, line.Length)):
			at = Advance(at, line[i])
		return at

	private static def Advance(column as int, c as char) as int:
		return column + 1 unless c == char('\t')
		return column + TabSize - ((column - 1) % TabSize)

	private static def Below(oneBased as int) as int:
		return 0 if oneBased < 1
		return oneBased - 1
