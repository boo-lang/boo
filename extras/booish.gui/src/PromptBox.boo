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
import System.Text
import System.IO
import System.Reflection
import System.Windows.Forms
import System.Drawing
import System.Runtime.InteropServices
import Boo.Lang.Interpreter

class KeyChar:
	
	public static Enter = chr(Keys.Enter)
	
	public static Dot = chr('.')
	
	public static Back = chr(Keys.Back)
	
	public static Esc = chr(Keys.Escape)
	
	public static Del = chr(Keys.Delete)

class CompletionBox(ListBox):
	
	_interpreter as InteractiveInterpreter
	
	_currentCodeCompletion = ""
	
	_codeCompletionArchive = []
	
	_upsideDown = false
	
	def constructor(interpreter):
		self._interpreter = interpreter		
		self.Visible = false
		
	def Fill(type as System.Type):		
		members = []
		for member in type.GetMembers():
			if IsValidSuggestion(member):
				if member isa FieldInfo:
					field = member as FieldInfo
					members.Add("${field.Name} as ${getBooTypeName(field.FieldType)}")
				elif member isa PropertyInfo:
					property = member as PropertyInfo
					getField = ("", "getter,")[property.CanRead]
					setField = ("", "setter")[property.CanWrite]
					members.Add("${member.Name} as ${getBooTypeName(property.PropertyType)} (${getField} ${setField})")
				elif member isa MethodInfo:
					method = member as MethodInfo
					params = join("${param.Name} as ${getBooTypeName(param.ParameterType)}"
										for param in method.GetParameters(), ", ")					
					returnValue = ""
					returnValue = " as ${getBooTypeName(method.ReturnType)}" if method.ReturnType is not void					
					members.Add("${member.Name}(${params})${returnValue}")
					
		Items.Clear()
		_codeCompletionArchive.Clear()
		for item in members.Sort():
			Items.Add(item) if not _upsideDown
			Items.Insert(0, item) if _upsideDown
			_codeCompletionArchive.Add(item)
		self.SelectedIndex = (0, self.Items.Count - 1)[_upsideDown and self.Items.Count > 1]
			
	def Show(pos as Point):
		//Ensure that this listbox won't extend past the bottom of the screen.
		//(Most common use-case.)
		self.Location = pos
		_upsideDown = self.Bottom > Parent.Bottom
		if _upsideDown:
			par = cast(PromptBox, Parent)
			//FIXME: Here's the deal - I don't know how to get the size of the caret,
			//So the code completion box will always be "just so" off
			//and prove to be visually distracting.
			//TODO: The FIXME above. ;)
			caretDis = Parent.Bottom - par.CaretPos.Y		
			caretDis -= self.Font.Height		
			self.Location =  Point(self.Location.X, (self.Location.Y - (self.Bottom - Parent.Bottom) ) - caretDis )
		 		
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
			_currentCodeCompletion = ""
			args.Handled = true	
		// Adjust listview to _currentCodeCompletion
		// Also, adjust to focus if _upsideDown!
		smartComplete = def():
			self.Items.Clear()
			self.Items.Add("${_currentCodeCompletion} (*)")
			for data as string in _codeCompletionArchive:
				if data.ToLower().StartsWith(_currentCodeCompletion.ToLower()):
					self.Items.Add(data) if not _upsideDown
					self.Items.Insert(0, data) if _upsideDown
			// Make sure one member is always selected by de funk.
			// Always show the user what he's been typing, meanwhile.
			self.SelectedIndex = (0, 1)[self.Items.Count > 1] if not _upsideDown
			self.SelectedIndex = (self.Items.Count - 1, self.Items.Count - 2)[self.Items.Count > 1] if _upsideDown
			
		if args.KeyChar in KeyChar.Esc, KeyChar.Back:
			par = cast(PromptBox, Parent)
			if args.KeyChar == KeyChar.Esc:
				par.Text += _currentCodeCompletion				
				par.SelectionStart = par.Text.Length
				finish()
			elif _currentCodeCompletion == "":				
				finish()
			else:
				_currentCodeCompletion = _currentCodeCompletion.Remove(
					_currentCodeCompletion.Length - 1, 1)				
				smartComplete()
		elif KeyChar.Enter == args.KeyChar or KeyChar.Dot == args.KeyChar:
			try:
				cast(TextBox, Parent).SelectedText = /\w+/.Match(SelectedItem).Groups[0].Value
			except e: //User did NOT select an item before triggering condition.
				pass
			ensure:
				finish()
		else:
			_currentCodeCompletion += args.KeyChar			
			smartComplete()
			super(args)
			
	private def getBooTypeName(type as System.Type):
		return _interpreter.getBooTypeName(type)
		

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
								
	_completionBox = CompletionBox(_interpreter)
	
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
		// These blocks of code:
		// *keeps the user from deleting the ">>>" and screwing up the interpretor.
		// *keeps the user from "going up" and editing already inputted text.		
		// Keep user from selecting-and-deleting.
		
		currentPrompt = Text.Length - (Lines[-1][4:].Length)		
		if SelectedText != "" and SelectionStart < currentPrompt:			
			args.Handled = true
		//Make sure user's cursor hasn't drifted beyond ">>> "
		elif SelectedText == "" and args.KeyChar == KeyChar.Back:			
			if SelectionStart <= currentPrompt:				
				args.Handled = true
		elif KeyChar.Enter == args.KeyChar:
			try:
				(SingleLineInputState, BlockInputState)[_state]()
			except x:				
				print(x)
			prompt()
			args.Handled = true			
		//print "Did we get this far, yet?"
		elif KeyChar.Dot == args.KeyChar:
			self.SelectedText = "."
			DotComplete()			
			args.Handled = true	
		
		super(args)
		
	override def OnResize(args as EventArgs):
		_completionBox.Size = System.Drawing.Size(Width*.65, Height*.4)
		super(args)
		
	def DotComplete():		
		m = /((\w|\.)+)\.$/.Match(self.Lines[-1])		
		if m.Success:
			// only simple identifiers right now
			expression = m.Groups[1].Value
			if /^\w+$/.IsMatch(expression):	
				type = _interpreter.Lookup(expression)
				if type is not null:					
					_completionBox.Show(CaretPos)	
					_completionBox.Fill(type)
		
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
