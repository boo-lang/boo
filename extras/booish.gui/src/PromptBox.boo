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

namespace booish.gui

import System
import System.IO
import System.Reflection
import booish
import System.Windows.Forms
import System.Drawing
import System.Runtime.InteropServices

class KeyChar:
	
	public static Enter = chr(Keys.Enter)
	
	public static Dot = chr('.')
	
	public static Back = chr(Keys.Back)
	
	public static Esc = chr(Keys.Escape)

class CompletionBox(ListBox):
	
	def constructor():
		self.Visible = false
		
	def Fill(type as System.Type):
		
		members = {}
		for member in type.GetMembers():
			if member.Name not in members:
				if IsValidSuggestion(member):
					members.Add(member.Name, member)
				
		Items.Clear()
		for item in List(members.Keys).Sort():
			Items.Add(item)
			
	def Show(pos as Point):
		self.Location = pos
		self.Visible = true
		self.Focus()
		
	private def IsValidSuggestion(member as MemberInfo):
		return false if not IsPublic(member)		
		method = member as MethodBase
		if method is not null:
			return false if method.IsSpecialName
		return true
		
	private def IsPublic(member as MemberInfo):
		if MemberTypes.Method == member.MemberType:
			return cast(MethodBase, member).IsPublic
		if MemberTypes.Property == member.MemberType:
			p as PropertyInfo = member
			return (p.GetGetMethod() or p.GetSetMethod()).IsPublic
		if MemberTypes.Field == member.MemberType:
			return cast(FieldInfo, member).IsPublic
		return true
	
	override def OnLostFocus(args as EventArgs):
		self.Visible = false
		super(args)
		
	override def OnKeyPress(args as KeyPressEventArgs):
		finish = def ():
			Parent.Focus()
			args.Handled = true
			
		if args.KeyChar in KeyChar.Esc, KeyChar.Back:
			finish()
		elif KeyChar.Enter == args.KeyChar:
			cast(TextBox, Parent).SelectedText = SelectedItem
			finish()
		else:			
			super(args)
		

class PromptBox(TextBox):
	
	enum InputState:
		SingleLine = 0
		Block = 1
		
	_state = InputState.SingleLine
	
	_block = System.IO.StringWriter()
	
	_console = StringWriter()
	
	[getter(Interpreter)]
	_interpreter = InteractiveInterpreter(
								RememberLastValue: true,
								Print: print)
								
	_completionBox = CompletionBox()
	
	def constructor():
		self.Dock = DockStyle.Fill
		self.Multiline = true
		self.AcceptsTab = true
		self.ScrollBars = ScrollBars.Vertical
		_interpreter.References.Add(typeof(TextBox).Assembly)
		_interpreter.References.Add(typeof(Font).Assembly)
		_interpreter.SetValue("inspect", inspect)
		
		Controls.Add(_completionBox)
		
		prompt()
		
	CaretPos:
		get:
			p as Point
			assert booish.gui.externs.User32.GetCaretPos(p)
			return p
		
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
			Eval(code)
		
	def BlockInputState():
		code = GetCurrentLine()
		if 0 == len(code):
			Eval(_block.ToString())
			_state = InputState.SingleLine
		else:
			_block.WriteLine(code)
		
	override def OnKeyPress(args as KeyPressEventArgs):
		if KeyChar.Enter == args.KeyChar:			
			try:
				(SingleLineInputState, BlockInputState)[_state]()
			except x:				
				print(x)
			prompt()
			args.Handled = true
		elif KeyChar.Dot == args.KeyChar:
			self.SelectedText = "."
			DotComplete()
			args.Handled = true
			
		super(args)
		
	override def OnResize(args as EventArgs):
		_completionBox.Size = System.Drawing.Size(Width*.5, Height*.4)
		super(args)
		
	def DotComplete():		
		m = /((\w|\.)+)\.$/.Match(self.Lines[-1])		
		if m.Success:
			// only simple identifiers right now
			expression = m.Groups[1].Value
			if /^\w+$/.IsMatch(expression):	
				type = _interpreter.Lookup(expression)
				if type is not null:
					_completionBox.Fill(type)
					_completionBox.Show(CaretPos)	
		
	def Eval(code as string):
		saved = Console.Out
		Console.SetOut(_console)
		try:
			_interpreter.LoopEval(code)			
		ensure:
			FlushConsole()
			Console.SetOut(saved)
			
	def FlushConsole():
		AppendText(_console.ToString())
		_console.GetStringBuilder().Length = 0
			
	def print(msg):
		AppendText("${msg}\r\n")
		
	def inspect([required] obj):
		f = Form(Text: "Object Inspector [${obj}]")
		f.Controls.Add(PropertyGrid(
							Dock: DockStyle.Fill,
							SelectedObject: obj,
							Font: Font))
		f.Show()
		return f
			
	def prompt():
		AppendText((">>> ", "... ")[_state])
		
def chr(key as Keys):
	return cast(IConvertible, cast(int, key)).ToChar(null)
		
def chr(s as string):
	assert len(s) == 1
	return s[0]
