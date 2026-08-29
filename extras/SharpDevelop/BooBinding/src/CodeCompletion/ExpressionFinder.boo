#region license
// Copyright (c) 2004, Daniel Grunwald (daniel@danielgrunwald.de)
// All rights reserved.
//
// BooBinding is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// BooBinding is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with BooBinding; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

namespace BooBinding.CodeCompletion

import System
import System.Text
import SharpDevelop.Internal.Parser

class ExpressionFinder(IExpressionFinder):
	// The expression finder can find an expression in a text
	// inText is the full source code, offset the cursor position
	
	// example: "_var = 'bla'\n_var^\nprint _var"
	// where ^ is the cursor position
	// in that simple case the expression finder should return 'n_var'.
	
	// but also complex expressions like
	// 'filename.Substring(filename.IndexOf("var="))'
	// should be returned if the cursor is after the last ).
	
	// implementation note: the text after offset is irrelevant, so
	// every operation on the string aborts after reaching offset
	
	final static _closingBrackets = '}])'
	final static _openingBrackets = '{[('
	
	def FindExpression(inText as string, offset as int) as string:
		return null if inText == null
		print "Trying quickfind for ${offset}"
		// OK, first try a kind of "quick find"
		i = offset + 1
		forbidden = '"\'/#)]}'
		finish = '([{=+*<,:'
		start = -1
		while i > 0:
			i -= 1
			c = inText[i]
			if finish.IndexOf(c) >= 0:
				start = i + 1
				break
			if forbidden.IndexOf(c) >= 0:
				print "Quickfind failed: got ${c}"
				break
			if Char.IsWhiteSpace(c):
				if i > 6 and inText.Substring(i - 6, 6) == "import":
					i -= 7 // include 'import' in the expression
				start = i + 1
				break
		if start >= 0:
			if CheckString(inText, start, '/#"\'', '\r\n'):
				return GetExpression(inText, start, offset + 1)
		
		inText = SimplifyCode(inText, offset)
		if inText == null:
			print 'SimplifyCode returned null (cursor is in comment/string???)'
			return null
		// inText now has no comments or string literals, but the same meaning in
		// terms of the type system
		// Now go back until a finish-character or a whitespace character
		bracketStack = StringBuilder() // use Stack<char> instead in .NET 2.0
		i = inText.Length
		while i > 0:
			i -= 1
			c = inText[i]
			if bracketStack.Length == 0 and (finish.IndexOf(c) >= 0 or Char.IsWhiteSpace(c)):
				return GetExpression(inText, i + 1, inText.Length)
			if _closingBrackets.IndexOf(c) >= 0:
				bracketStack.Append(c)
			bracket = _openingBrackets.IndexOf(c)
			if bracket >= 0:
				while Pop(bracketStack) > bracket:
					pass
		
		return null
	
	private def CheckString(text as string, offset as int, forbidden as string, finish as string):
		i = offset
		while i > 0:
			i -= 1
			c = text[i]
			return false if forbidden.IndexOf(c) >= 0
			return true if finish.IndexOf(c) >= 0
		return true
	
	private def Pop(bracketStack as StringBuilder):
		return -1 if bracketStack.Length == 0
		c = bracketStack[bracketStack.Length - 1]
		bracketStack.Length -= 1
		return _closingBrackets.IndexOf(c)
	
	private def GetExpression(inText as string, start as int, end as int):
		b = StringBuilder()
		wasSpace = true
		i = start
		while i < end:
			c = inText[i]
			if Char.IsWhiteSpace(c):
				b.Append(' ') unless wasSpace
				wasSpace = true
			else:
				wasSpace = false
				b.Append(c)
			i += 1
		print "Expression is '${b}'"
		return b.ToString().Trim()
	
	// TODO: We could need some unit tests for this.
	
	final static _elseIndex = 10
	
	static _stateTable = ( // "    '    \    \n   $    {    }    #    /    *   else
	/* 0: in Code       */  ( 1  , 7  , 0  , 0  , 0  , 0  , 0  , 13 , 12 , 0  , 0  ),
	/* 1: after "       */  ( 2  , 6  , 10 , 0  , 8  , 6  , 6  , 6  , 6  , 6  , 6  ),
	/* 2: after ""      */  ( 3  , 7  , 0  , 0  , 0  , 0  , 0  , 13 , 12 , 0  , 0  ),
	/* 3: in """        */  ( 4  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  ),
	/* 4: in """, "     */  ( 5  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  ),
	/* 5: in """, ""    */  ( 0  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  ),
	/* 6: in "-string   */  ( 0  , 6  , 10 , 0  , 8  , 6  , 6  , 6  , 6  , 6  , 6  ),
	/* 7: in '-string   */  ( 7  , 0  , 11 , 0  , 7  , 7  , 7  , 7  , 7  , 7  , 7  ),
	/* 8: after $ in "  */  ( 0  , 6  , 10 , 0  , 8  , 9  , 6  , 6  , 6  , 6  , 6  ),
	/* 9: in "{         */  ( 9  , 9  , 9  , 9  , 9  , 9  , 6  , 9  , 9  , 9  , 9  ),
	/* 10: after \ in " */  ( 6  , 6  , 6  , 0  , 6  , 6  , 6  , 6  , 6  , 6  , 6  ),
	/* 11: after \ in ' */  ( 7  , 7  , 7  , 0  , 7  , 7  , 7  , 7  , 7  , 7  , 7  ),
	/* 12: after /      */  ( 1  , 7  , 0  , 0  , 0  , 0  , 0  , 0  , 13 ,-14 , 0  ),
	/* 13: line comment */  ( 13 , 13 , 13 , 0  , 13 , 13 , 13 , 13 , 13 , 13 , 13 ),
	/* 14: block comment*/  ( 14 , 14 , 14 , 14 , 14 , 14 , 14 , 14 , 14 , 15 , 14 ),
	/* 15: after * in bc*/  ( 14 , 14 , 14 , 14 , 14 , 14 , 14 , 14 ,-15 , 15 , 14 )
	                     )
	
	def SimplifyCode(inText as string, offset as int):
	"""This method makes boo source code "simpler" by removing all comments
		and replacing all string litarals through string.Empty.
		Regular expressions literals are replaced with the simple regex /a/"""
		result = StringBuilder()
		inStringResult = StringBuilder(' ')
		state = 0
		commentblocks = 0
		inputTable = array(int, 128)
		for i in range(128):
			inputTable[i] = _elseIndex
		inputTable[ 34] = 0 // "
		inputTable[ 39] = 1 // '
		inputTable[ 92] = 2 // \
		inputTable[ 10] = 3 // \n
		inputTable[ 13] = 3 // \r
		inputTable[ 36] = 4 // $
		inputTable[123] = 5 // {
		inputTable[125] = 6 // }
		inputTable[ 35] = 7 // #
		inputTable[ 47] = 8 // /
		inputTable[ 42] = 9 // *
		i = -1
		while i < offset:
			i += 1
			c as char = inText[i]
			charNum as int = cast(int, c)
			if charNum > 127:
				input = _elseIndex
			else:
				input = inputTable[charNum]
			action = _stateTable[state][input]
			if action == -14:
				// enter block comment
				commentblocks += 1
				state = 14
			elif action == -15:
				// leave block comment
				commentblocks -= 1
				if commentblocks == 0:
					state = 0
				else:
					state = 14
			elif action == 9:
				// enter inner string expression (${...})
				if state == 9:
					inStringResult.Append(c)
				else:
					inStringResult.Length = 1
				state = action
			elif action == 0 or action == 12:
				// go to normal code
				if action == 12:
					// after / could be a regular expression, do a special check for that
					regexEnd = SkipRegularExpression(inText, i, offset)
					if regexEnd > 0:
						i = regexEnd
						result.Append('/a')
					elif regexEnd == -1:
						// cursor is in regex
						return null
				if state == 2 or (state >= 6 and state <= 11):
					result.Append("string.Empty")
				if state == 0 or state == 2 or state == 12:
					result.Append(c)
				state = action
			else:
				state = action
		if state == 0 or state == 2 or state == 12:
			// cursor is in normal code
			return result.ToString()
		elif state == 9:
			// cursor is in inner string expression (${...})
			return inStringResult.ToString()
		else:
			// cursor is in comment/string
			return null
	
	final static slashChar = '/'[0]
	
	def SkipRegularExpression(inText as string, pos as int, maxOffset as int):
	"""Skips the regular expression in inText at position pos. Returns end position of the ending / if
	successful or 0 is no regular expression was found at the location.
	Return -1 if maxOffset is inside the regular expression."""
		if pos > 0:
			containsWhitespace = (inText[pos - 1] == '@'[0])
		else:
			containsWhitespace = false
		return -1 if pos == maxOffset             // cursor is after / -> cursor inside regex
		return 0 if inText[pos + 1] == slashChar // double // is comment, no regex
		i = pos
		while i < maxOffset:
			i += 1
			if (not containsWhitespace) and Char.IsWhiteSpace(inText, i):
				return 0; // this is no regex
			return i if inText[i] == slashChar
		return -1 // maxOffset inside regex
