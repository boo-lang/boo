namespace BooExplorer

import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
import ICSharpCode.TextEditor.Actions

import System.Windows.Forms

class TextManipulator:
	_editor as TextEditorControl

	def constructor(editor as TextEditorControl):
		_editor = editor

	def Insert(nextText as string):
		newCaretOffset = _editor.ActiveTextAreaControl.TextArea.Caret.Offset
		_editor.Document.Insert(newCaretOffset, nextText)

	def GetWordBeforeCaret() as string:
		start = TextUtilities.FindPrevWordStart(_editor.Document, _editor.ActiveTextAreaControl.TextArea.Caret.Offset);
		return _editor.Document.GetText(start, _editor.ActiveTextAreaControl.TextArea.Caret.Offset - start);
