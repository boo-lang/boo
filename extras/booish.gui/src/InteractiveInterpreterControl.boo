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

"""
Interactive Forms-based Console
"""
namespace booish.gui

import System
import System.Drawing
import System.IO
import System.Windows.Forms
import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
import ICSharpCode.TextEditor.Actions
import ICSharpCode.TextEditor.Gui.CompletionWindow
import Boo.Lang.Compiler.TypeSystem

interface ICompletionWindowImageProvider:		
	ImageList as ImageList:
		get
	def GetImageIndex(entity as IEntity) as int

class InteractiveInterpreterControl(TextEditorControl):
	
	enum InputState:
		SingleLine = 0
		Block = 1
		
	class NullCompletionWindowImageProvider(ICompletionWindowImageProvider):
		
		public static final Instance = NullCompletionWindowImageProvider()
		
		[getter(ImageList)]
		_imageList = System.Windows.Forms.ImageList()
		
		def GetImageIndex(entity as IEntity) as int:
			return 0

		
	_state = InputState.SingleLine
	
	_block = System.IO.StringWriter()
	
	_console = System.IO.StringWriter()
	
	[getter(Interpreter)]
	_interpreter as Boo.Lang.Interpreter.InteractiveInterpreter

	_codeCompletionWindow as CodeCompletionWindow
	
	[property(CompletionWindowImageProvider, value is not null)]
	_imageProvider as ICompletionWindowImageProvider = NullCompletionWindowImageProvider()
	
	def constructor():
		self._interpreter = Boo.Lang.Interpreter.InteractiveInterpreter(
								RememberLastValue: true,
								Print: print)
		self.Document.HighlightingStrategy = GetBooHighlighting()
		self.EnableFolding =  false
		self.ShowLineNumbers =  false
		self.ShowSpaces = true
		self.ShowTabs =  true
		self.ShowEOLMarkers = false
		self.ShowInvalidLines = false
		self.Dock = DockStyle.Fill
		
	CaretColumn:
		get:
			return self.ActiveTextAreaControl.Caret.Column
			
	CurrentLineText:
		get:
			segment = GetLastLineSegment()
			return self.Document.GetText(segment)[4:]	
		
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
		
	def ConsumeCurrentLine():		
		text = CurrentLineText
		print("")
		return text
		
	def GetLastLineSegment():
		return self.Document.GetLineSegment(self.Document.LineSegmentCollection.Count)
		
	def SingleLineInputState():
		code = ConsumeCurrentLine()
		
		if code[-1:] in ":", "\\":
			_state = InputState.Block
			_block.GetStringBuilder().Length = 0
			_block.WriteLine(code)
			prompt()
		else:
			Eval(code)
		
	def BlockInputState():
		code = ConsumeCurrentLine()
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

	override def InitializeTextAreaControl(newControl as TextAreaControl):
		super(newControl)
		newControl.TextArea.DoProcessDialogKey += HandleDialogKey
		newControl.TextArea.KeyEventHandler += HandleKeyPress
		
	InCodeCompletion:
		get:
			return _codeCompletionWindow is not null and not _codeCompletionWindow.IsDisposed

	private def CodeComplete(ch as System.Char):
		_codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow(
					self.ParentForm, 
					self, 
					"<code>",
					CodeCompletionDataProvider(_imageProvider, GetSuggestions()), 
					ch)
					
	private def GetSuggestions():
		suggestion = _interpreter.SuggestCodeCompletion(CurrentLineText + ".__codecomplete__")
		return array(IEntity, 0) if suggestion is null
		return (suggestion as INamespace).GetMembers()
		
	private def HandleDialogKey(key as Keys):
		return false if InCodeCompletion
		
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
		
	private def HandleKeyPress(ch as System.Char) as bool:
		if InCodeCompletion:
			_codeCompletionWindow.ProcessKeyEvent(ch)

		if ch == "."[0]:
			CodeComplete(ch)
			return false

		return false
		
	def GetBooHighlighting():
		return HighlightingManager.Manager.FindHighlighter("Boo")

