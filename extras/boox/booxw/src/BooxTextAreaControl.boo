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

		for interceptor in Editor.Main.TextInterceptors:
			ret = interceptor.Process(ch, TextManipulator(self))
			break unless ret

		return false

	def CodeComplete(ch as System.Char):
		_codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow(
					Editor.Main, 
					self, 
					Editor.GetSafeFileName(),
					CodeCompletionDataProvider(CodeCompletionHunter.GetCompletion(GetCompletionSource())), 
					ch)

	private def GetCompletionSource():
		newCaretOffset = self.ActiveTextAreaControl.TextArea.Caret.Offset
		return self.Document.TextContent.Insert(newCaretOffset, ".__codecomplete__")
