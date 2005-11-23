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

import System
import Gtk from "gtk-sharp"
import Gdk from "gdk-sharp" as Gdk
import Pango from "pango-sharp" as Pango
import Boo.Lang.Interpreter from Boo.Lang.Interpreter

class PromptView(TextView):
	
	_interpreter = InteractiveInterpreter(RememberLastValue: true, Print: print)
	
	def constructor():
		
		self.WrapMode = Gtk.WrapMode.Word
		
		//if Environment.OSVersion.Platform in (PlatformID.Win32NT, PlatformID.Win32Windows):
		self.ModifyFont(Pango.FontDescription(Family: "Lucida Console"))
			
		_interpreter.References.Add(typeof(TextView).Assembly)
		_interpreter.References.Add(typeof(Gdk.Key).Assembly)
		
		_interpreter.SetValue("cls", { Buffer.Text = "" })
		_interpreter.SetValue("view", self)
		
		prompt()
		
	override def OnKeyPressEvent(ev as Gdk.EventKey):
		if Gdk.Key.Return == ev.Key:
			try:			
				EvalCurrentLine()
			except x:
				print(x)
			prompt()
			return true
		elif ev.Key in (Gdk.Key.BackSpace, Gdk.Key.Left):
			if Buffer.GetIterAtMark(Buffer.InsertMark).LineOffset < 5:
				return true
			
		return super(ev)
		
	def print(obj):
		Buffer.InsertAtCursor("${obj}\n")
		
	def prompt():
		Buffer.MoveMark(Buffer.InsertMark, Buffer.EndIter)
		Buffer.InsertAtCursor(">>> ")
		ScrollMarkOnscreen(Buffer.InsertMark)
		
	def EvalCurrentLine():
		start = Buffer.GetIterAtLine(Buffer.LineCount)
		line = Buffer.GetText(start, Buffer.EndIter, false)
			
		print("")
		_interpreter.LoopEval(line[4:])	

class MainWindow(Window):
	
	def constructor():
		super("booish")
	
		window = ScrolledWindow()
		window.Add(PromptView())
		
		self.Add(window)
		
		self.DeleteEvent += Application.Quit

Application.Init()

window = MainWindow(DefaultWidth:  400,
				DefaultHeight: 250)
				
window.ShowAll()

Application.Run()
