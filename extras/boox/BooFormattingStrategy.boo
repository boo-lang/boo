namespace BooExplorer

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
							indentation+document.GetText(currentLine))
			return len(indentation)
		
		return super(area, line)

