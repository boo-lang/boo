#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rodrigobamboo@gmail.com)
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

namespace BooBinding

import System
import System.Drawing
import System.Windows.Forms
import ICSharpCode.SharpDevelop.Gui
import ICSharpCode.Core.Services
import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
import ICSharpCode.TextEditor.Actions
import ICSharpCode.TextEditor.Gui.CompletionWindow

class BooPromptControl(TextEditorControl):
	
	enum InputState:
		SingleLine = 0
		Block = 1
		
	_state = InputState.SingleLine
	
	_block = System.IO.StringWriter()
	
	_console = System.IO.StringWriter()
	
	[getter(Interpreter)]
	_interpreter = Boo.Lang.Interpreter.InteractiveInterpreter(
								RememberLastValue: true,
								Print: print)

	_codeCompletionWindow as CodeCompletionWindow
	
	def constructor():
		self.Document.HighlightingStrategy = GetBooHighlighting()
		self.Document.FormattingStrategy = BooFormattingStrategy()
		self.IndentStyle = IndentStyle.Smart
		self.EnableFolding =  false
		self.ShowLineNumbers = false
		self.ShowSpaces = true
		self.ShowTabs = true
		self.ShowEOLMarkers = false
		self.ShowInvalidLines = false
		
	CaretColumn:
		get:
			return self.ActiveTextAreaControl.Caret.Column
		
	override def OnLoad(args as EventArgs):
		super(args)
		prompt()
		
	def Eval(code as string):
		saved = Console.Out
		Console.SetOut(_console)
		try:
			_interpreter.LoopEval(code)			
		ensure:
			FlushConsole()
			Console.SetOut(saved)
			_state = InputState.SingleLine
			prompt()
			
	def FlushConsole():
		AppendText(_console.ToString())
		_console.GetStringBuilder().Length = 0
		
	def GetCurrentLine():		
		segment = GetLastLineSegment()
		text = self.Document.GetText(segment)[4:]
		print("")
		return text
		
	def GetLastLineSegment():
		return self.Document.GetLineSegment(self.Document.LineSegmentCollection.Count)
		
	private def SingleLineInputState():
		code = GetCurrentLine()
		
		if code[-1:] in ":", "\\":
			_state = InputState.Block
			_block.GetStringBuilder().Length = 0
			_block.WriteLine(code)
			prompt()
		else:
			Eval(code)
		
	private def BlockInputState():
		code = GetCurrentLine()
		if 0 == len(code):
			Eval(_block.ToString())			
		else:
			_block.WriteLine(code)
			prompt()
			
	def print(msg):
		AppendText("${msg}\r\n")		
				
	def prompt():
		AppendText((">>> ", "... ")[_state])
		
	def AppendText(text as string):
		segment = GetLastLineSegment()
		self.Document.Insert(segment.Offset + segment.TotalLength, text)
		MoveCaretToEnd()
		
	def MoveCaretToEnd():
		segment = GetLastLineSegment()
		newOffset = segment.Offset + segment.TotalLength
		self.ActiveTextAreaControl.Caret.Position = self.Document.OffsetToPosition(newOffset)

	override protected def InitializeTextAreaControl(newControl as TextAreaControl):
		super(newControl)
		newControl.TextArea.DoProcessDialogKey += HandleDialogKey
		
	private def HandleDialogKey(key as Keys):
		if key == Keys.Enter:
			try:
				(SingleLineInputState, BlockInputState)[_state]()
			except x:				
				print(x)
			return true
			
		if key in Keys.Back, Keys.Left:
			if self.CaretColumn < 5:
				return true
		else:
			if self.CaretColumn < 4:
				MoveCaretToEnd()
				
		return false
		
	private def GetBooHighlighting():
		return HighlightingManager.Manager.FindHighlighter("Boo")

class BooishView(AbstractPadContent):
	
	_box = BooPromptControl(Font: System.Drawing.Font("Lucida Console", 10))
	
	def constructor():
		super("booish")
		
		_box.Interpreter.SetValue("Workbench", WorkbenchSingleton.Workbench)
		
	override Control as Control:
		get:
			return _box

