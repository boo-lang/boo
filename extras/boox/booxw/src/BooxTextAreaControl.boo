namespace BooExplorer

import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
import ICSharpCode.TextEditor.Actions
import ICSharpCode.TextEditor.Gui.CompletionWindow

import BooExplorer.Common

class BooxTextAreaControl(TextEditorControl):

	[property(Editor)]
	_editor as BooEditor

	_codeCompletionWindow as CodeCompletionWindow

	override def InitializeTextAreaControl(newControl as TextAreaControl):
		super(newControl)
		newControl.TextArea.KeyEventHandler += HandleKeyPress

	def HandleKeyPress(ch as System.Char) as bool:
		if _codeCompletionWindow is not null and not _codeCompletionWindow.IsDisposed:
			_codeCompletionWindow.ProcessKeyEvent(ch)

		if ch == "."[0]:
			CodeComplete(ch)
			return false

		return false

	def CodeComplete(ch as System.Char):
		_codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow(
					Editor.Main, 
					self, 
					Editor.GetSafeFileName(),
					CodeCompletionDataProvider(CodeCompletion(GetCompletionSource())), 
					ch)

	private def GetCompletionSource():
		newCaretOffset = self.ActiveTextAreaControl.TextArea.Caret.Offset
		return self.Document.TextContent.Insert(newCaretOffset, ".__codecomplete__")
