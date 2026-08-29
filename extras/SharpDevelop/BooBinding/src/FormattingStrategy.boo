#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// This file is part of Boo Explorer.
//
// Boo Explorer is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// Boo Explorer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

namespace BooBinding

import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
import ICSharpCode.TextEditor.Actions

class BooFormattingStrategy(DefaultFormattingStrategy):
	override def SmartIndentLine(area as TextArea, line as int) as int:
		document = area.Document
		previousLine = document.GetLineSegment(line-1)
		
		if document.GetText(previousLine).EndsWith(":"):
			currentLine = document.GetLineSegment(line)
			indentation = GetIndentation(area, line-1)
			indentation += Tab.GetIndentationString(document)
			document.Replace(currentLine.Offset,
			                 currentLine.Length,
			                 indentation + document.GetText(currentLine))
			return len(indentation)
		
		return super(area, line)
	
	// Deactivate indenting multiple lines with Ctrl-I
	override def IndentLines(textArea as TextArea, begin as int, end as int):
		pass
