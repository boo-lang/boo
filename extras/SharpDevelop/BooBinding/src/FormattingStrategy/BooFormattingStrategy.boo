namespace BooBinding.FormattingStrategy

import System

import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document

class BooFormattingStrategy(DefaultFormattingStrategy):
""" This class handle the auto and smart indenting in the textbuffer while you type.
"""
	protected override def SmartIndentLine(textArea as TextArea, lineNr as int) as int:
	"""Boo specific smart indenting for a line :)"""
		
		if lineNr > 0:
			lineAbove = textArea.Document.GetLineSegment(lineNr - 1)
			lineAboveText = textArea.Document.GetText(lineAbove.Offset, lineAbove.Length).Trim()

			curLine = textArea.Document.GetLineSegment(lineNr)
			curLineText = textArea.Document.GetText(curLine.Offset, curLine.Length).Trim()

			if lineAboveText.EndsWith(":"):
				indentation = GetIndentation(textArea, lineNr - 1)
				textArea.Document.Replace(curLine.Offset, curLine.Length, indentation + curLineText)
				return indentation.Length

		return AutoIndentLine(textArea, lineNr)
