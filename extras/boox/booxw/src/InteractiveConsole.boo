#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

"""
Interactive Forms-based Console
"""

namespace BooExplorer

import System
import booish
import System.Windows.Forms
import System.Drawing
import WeifenLuo.WinFormsUI

class InteractiveConsole(DockContent):
	
	def constructor(mainForm as MainForm):
	
		self.Text = "Interactive Console"
		self.Controls.Add(console=PromptBox())
					
		console.Interpreter.SetValue("MainForm", mainForm)
		console.Interpreter.SetValue("inspect", inspect)
		
	def inspect([required] obj):
		f = Form(Text: "Object Inspector [${obj.ToString()[:10]}...]")
		f.Controls.Add(PropertyGrid(
							Dock: DockStyle.Fill,
							SelectedObject: obj,
							Font: Font,
							PropertySort: PropertySort.Alphabetical))
		f.Show()
		return f
		
	
	override def GetPersistString():
		return "InteractiveConsole|"

class PromptBox(TextBox):
	
	static Enter = chr(13)
	
	enum InputState:
		SingleLine = 0
		Block = 1
		
	_state = InputState.SingleLine
	
	_block = System.IO.StringWriter()
	
	[getter(Interpreter)]
	_interpreter = InteractiveInterpreter(
								RememberLastValue: true,
								Print: print)
	
	def constructor():
		self.Font = System.Drawing.Font("Lucida Console", 12)
		self.Dock = DockStyle.Fill
		self.Multiline = true
		self.AcceptsTab = true
		self.ScrollBars = ScrollBars.Vertical
		_interpreter.References.Add(typeof(TextBox).Assembly)
		_interpreter.References.Add(typeof(Font).Assembly)
		
		prompt()
		
	def GetCurrentLine():
		line = Lines[-1][4:]	
		print("")
		return line
		
	def SingleLineInputState():
		code = GetCurrentLine()
		
		if code[-1:] in ":", "\\":
			_state = InputState.Block
			_block.GetStringBuilder().Length = 0
			_block.WriteLine(code)
		else:
			_interpreter.LoopEval(code)
		
	def BlockInputState():
		code = GetCurrentLine()
		if 0 == len(code):
			_interpreter.LoopEval(_block.ToString())
			_state = InputState.SingleLine
		else:
			_block.WriteLine(code)
		
	override def OnKeyPress(args as KeyPressEventArgs):
		if Enter == args.KeyChar:			
			try:
				(SingleLineInputState, BlockInputState)[_state]()
			except x:				
				print(x)
			prompt()
			args.Handled = true
		super(args)
			
	def print(msg):
		AppendText("${msg}\r\n")
			
	def prompt():
		AppendText((">>> ", "... ")[_state])
		
	static def chr(value as int):
		return cast(IConvertible, value).ToChar(null)


