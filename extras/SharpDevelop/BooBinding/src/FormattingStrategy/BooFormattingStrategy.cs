using System;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace BooBinding.FormattingStrategy
{
	// This class handle the auto and smart indenting in the textbuffer while you type.
	public class BooFormattingStrategy : DefaultFormattingStrategy
	{
		// Boo specific smart indenting for a line :)
		protected override int SmartIndentLine(TextArea textArea, int lineNr)
		{
			if (lineNr > 0)
			{
				LineSegment lineAbove = textArea.Document.GetLineSegment(lineNr - 1);
				string lineAboveText = textArea.Document.GetText(lineAbove.Offset, lineAbove.Length).Trim();

				LineSegment curLine = textArea.Document.GetLineSegment(lineNr);
				string curLineText = textArea.Document.GetText(curLine.Offset, curLine.Length).Trim();
	
				if (lineAboveText.EndsWith(":"))
				{
					string indentation = GetIndentation(textArea, lineNr - 1);
					indentation += ICSharpCode.TextEditor.Actions.Tab.GetIndentationString(textArea.Document);
					textArea.Document.Replace(curLine.Offset, curLine.Length, indentation + curLineText);
					return indentation.Length;
				}
			}

			return AutoIndentLine(textArea, lineNr);
		}
	}
}
