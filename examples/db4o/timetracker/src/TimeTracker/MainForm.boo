#region license
// Copyright (c) 2003, 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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
This example shows how to use db4o 3.0 (http://www.db4o.com), a great
oodbm infrastructure, to implement a simple time tracking application.
"""

namespace TimeTracker

import System
import System.ComponentModel
import System.IO
import System.Drawing
import System.Windows.Forms
import TimeTracker.Model

class MainForm(Form):	
					
	_system as TimeTrackerSystem
	
	_components = System.ComponentModel.Container()
	
	_current as Activity
	
	_tabs = TabControl(Dock: DockStyle.Fill)
	
	_view = ListView(View: View.Details, Dock: DockStyle.Fill)
	
	_timer = System.Windows.Forms.Timer(_components,
										Tick: UpdateNotifyText,
										Interval: 30000)
										
	_notify as NotifyIcon
	
	def constructor():
		self.Text = "Boo Time Tracker (powered by db4o)"
		self.ShowInTaskbar = false		
		self.MinimizeBox = false
		Minimize()

		_notify = NotifyIcon(_components,
				Text: self.Text,
				Icon: self.Icon,
				Visible: true,
				ContextMenu: CreateContextMenu(),
				DoubleClick: { self.WindowState = WindowState.Normal })		
		
		_system = TimeTrackerSystem(
						Path.Combine(
							Application.UserAppDataPath,
							"timetracker.yap"))
							
		activitiesPage = TabPage(Text: "Activities")
		activitiesPage.Controls.Add(_view)
		
		prompt = booish.gui.PromptBox(
						Font: System.Drawing.Font("Lucida Console", 11))
		prompt.Interpreter.SetValue("system", _system)
		prompt.Interpreter.SetValue("MainForm", self)
		prompt.Interpreter.References.Add(typeof(Project).Assembly)
		promptPage = TabPage(Text: "Console")		
		promptPage.Controls.Add(prompt)
		_tabs.TabPages.AddRange((activitiesPage, promptPage))
			
		self.Controls.Add(_tabs)
		
		_timer.Start()
		
	def UpdateNotifyText():
		if _current is null:
			_notify.Text = self.Text
		else:
			_current.Finished = date.Now
			_notify.Text = _current.ToString()
		
	override def OnClosing(args as CancelEventArgs):
		unless Environment.HasShutdownStarted:
			Minimize()
			args.Cancel = true		
		super(args)
		
	def Minimize():		
		self.WindowState = WindowState.Minimized
							
	def _contextMenu_Popup(sender as ContextMenu):
		
		sender.MenuItems.Clear()
		
		for project in _system.Projects:
			sender.MenuItems.Add(CreateProjectMenuItem(project))
		 
		sender.MenuItems.Add(MenuItem("-"))
		sender.MenuItems.Add(
			MenuItem(Text: "New Project",
					Click: NewProject))
		sender.MenuItems.Add(MenuItem("-"))
		sender.MenuItems.Add(
			MenuItem(Text: "Quit",
					Click: Application.Exit))
					
	def CreateProjectMenuItem(project as Project):
		menu = MenuItem(Text: project.Name)
		for t in _system.QueryTasks(project):
			menu.MenuItems.Add(
				MenuItem(Text: t.Name, Click: { StartTask(t) }))
				
		menu.MenuItems.Add(MenuItem("-"))
		menu.MenuItems.Add(
			MenuItem(Text: "New Task",
					Click: { NewTask(project) }))
		return menu
							
	def CreateContextMenu():
		return System.Windows.Forms.ContextMenu(Popup: _contextMenu_Popup)
		
	def FlushCurrentActivity():
		 return if _current is null
		 
		 _current.Finished = date.Now
		 _system.AddActivity(_current)
		 
		 _current = null
		 
	def StartTask(task as Task):
		FlushCurrentActivity()		
		_current = Activity(Task: task, Started: date.Now)
		UpdateNotifyText()
		
	def NewTask(project as Project):
		name = Prompt("New Task", "Task Name: ")
		if name is not null:
			_system.AddTask(Task(Project: project, Name: name))
		
	def NewProject():
		name = Prompt("New Project", "Project's Name: ")
		if name is not null:
			_system.AddProject(Project(Name: name))
			
	def Prompt(title as string, message as string):
		dlg = PromptDialog(Text: title,
						Message: message,
						Font: self.Font,
						StartPosition: FormStartPosition.CenterScreen)
		if DialogResult.OK == dlg.ShowDialog():
			return dlg.Value
			
	override def Dispose(disposing as bool):
		FlushCurrentActivity()
		_components.Dispose()
		_system.Dispose()
		super(disposing)
		
class PromptDialog(Form):
	
	_value as TextBox
	_message as Label
	
	def constructor():		
		
		_message = Label(Location: Point(2, 2),
						Size: System.Drawing.Size(200, 18))
		_value = TextBox(
						Location: Point(2, 20),
						Size: System.Drawing.Size(290, 18))
						
		ok = Button(Text: "OK",
					Location: Point(50, 45),
					DialogResult: DialogResult.OK)
					
		cancel = Button(Text: "Cancel",
					Location: Point(150, 45),
					DialogResult: DialogResult.Cancel)
		
		SuspendLayout()
		self.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		self.StartPosition = FormStartPosition.CenterParent
		self.Size = System.Drawing.Size(300, 120)
		self.AcceptButton = ok
		self.CancelButton = cancel
		Controls.Add(_message)
		Controls.Add(_value)		
		Controls.Add(ok)
		Controls.Add(cancel)
		ResumeLayout(false)
		
	Message as string:
		set:
			_message.Text = value
			
	Value:
		get:
			return _value.Text
		set:
			_value.Text = value
		
Application.Run(MainForm())



