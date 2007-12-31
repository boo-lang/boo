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

import System
import System.Environment
import System.IO
import System.ComponentModel
import System.Windows.Forms
import System.Drawing
import WeifenLuo.WinFormsUI
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.IO

class MainForm(Form):

	_dockPanel as DockPanel
	_status as StatusBar
	_statusPanel1 as StatusBarPanel
	_timer as Timer
	
	[property(IsQuitting)]
	_isQuitting = false
	
	[getter(Settings)]
	_settings as BooxSettings = LoadSettings()

	[getter(DocumentOutline)]
	_documentOutline = BooExplorer.DocumentOutline()

	[getter(TaskList)]
	_taskList = BooExplorer.TaskList(self)
	
	[getter(OutputPane)]
	_outputPane = BooExplorer.OutputPane()
	
	[getter(InteractiveConsole)]
	_interactiveConsole = BooExplorer.InteractiveConsole(self)

	[getter(TextInterceptors)]
	_textInterceptors as (ITextInterceptor)

	_argv as (string)

	_menuItemClose as MenuItem
	_menuItemSave as MenuItem
	_menuItemSaveAs as MenuItem
	
	_parser = BooCompiler()
	
	_resourceManager = System.Resources.ResourceManager(MainForm)	
	
	_container = System.ComponentModel.Container()

	def constructor(argv as (string)):
		_argv = argv
		_dockPanel = DockPanel(Dock: DockStyle.Fill,
						ActiveAutoHideContent: null,
						TabIndex: 1,
						ActiveDocumentChanged: _dockPanel_ActiveDocumentChanged)

		_statusPanel1 = StatusBarPanel(AutoSize: StatusBarPanelAutoSize.Contents)

		_status = StatusBar(ShowPanels: true, TabIndex: 2)
		_status.Panels.Add(_statusPanel1)
		
		_parser.Parameters.Pipeline = Boo.Lang.Compiler.Pipelines.Parse()
		(_parser.Parameters.Pipeline[0] as duck).TabSize = 1		

		SuspendLayout()

		self.Icon = _resourceManager.GetObject("_icon")
		self.Size = System.Drawing.Size(800, 600)
		self.Menu = CreateMainMenu()
		self.Text = "Boo Explorer"
		self.IsMdiContainer = true

		_container.Add(_interactiveConsole)
		_container.Add(_documentOutline)
		_container.Add(_taskList)
		_container.Add(_outputPane)
		_container.Add(_dockPanel)
		_container.Add(_status)
		
		Controls.AddRange((
					_dockPanel,
					_status))
		ResumeLayout(false)
		
		LoadInterceptors()
		
		_timer = Timer(Tick: _timer_Tick, Interval: 50ms.TotalMilliseconds)
		_timer.Enabled = true
		
	override def Dispose(flag as bool):		
		SaveDockState()
		super(flag)
		
	private def GetSettingsFileName():
		return Path.Combine(GetApplicationDataFolder(), "settings.xml")
		
	private def SaveSettings():		
		_settings.Save(GetSettingsFileName())
		
	private def LoadSettings():
		fname = GetSettingsFileName()
		if File.Exists(fname):
			return BooxSettings.Load(fname)
		return BooxSettings()

	private def CreateMainMenu():
		menu = MainMenu()
		file = MenuItem(Text: "&File", MergeOrder: 0)
		file.MenuItems.Add(MenuItem(Text: "&Open...",
									Click: _menuItemOpen_Click,
									Shortcut: Shortcut.CtrlO))
		file.MenuItems.Add(MenuItem(Text: "&New",
									Click: _menuItemNew_Click,
									Shortcut: Shortcut.CtrlN))

		file.MenuItems.Add(MenuItem("-"))

		file.MenuItems.Add(_menuItemSave = MenuItem(Text: "&Save",
									Enabled: false,
									Click: _menuItemSave_Click,
									Shortcut: Shortcut.CtrlS))
		file.MenuItems.Add(_menuItemSaveAs = MenuItem(Text: "S&ave as...",
									Enabled: false,
									Click: _menuItemSaveAs_Click))

		file.MenuItems.Add(_menuItemClose = MenuItem(Text: "&Close",
									Click: _menuItemClose_Click,
									Shortcut: Shortcut.CtrlW,
									Enabled: false))

		file.MenuItems.Add(MenuItem("-"))
		file.MenuItems.Add(MenuItem(Text: "E&xit",
									Shortcut: Shortcut.CtrlQ,
									Click: _menuItemExit_Click))
									
		tools = MenuItem(Text: "&Tools", MergeOrder: 2, MergeType: MenuMerge.MergeItems)
		tools.MenuItems.Add(MenuItem(Text: "&Options",
								Shortcut: Shortcut.CtrlO,
								Click: _menuItemOptions_Click,
								MergeOrder: int.MaxValue))

		view = MenuItem(Text: "&View", MergeOrder: 4)
		view.MenuItems.AddRange(
			(
				MenuItem(Text: "Document Outline",
						Click: _menuItemDocumentOutline_Click,
						Shortcut: Shortcut.CtrlShiftD),
				MenuItem(Text: "Task List",
						Click: _menuItemTaskList_Click,
						Shortcut: Shortcut.CtrlShiftT),
				MenuItem(Text: "Output",
						Click: _menuItemOutputPane_Click,
						Shortcut: Shortcut.CtrlShiftO),
				MenuItem(Text: "Prompt",
						Click: ShowPrompt, 
						Shortcut: Shortcut.CtrlShiftP)
			))


		menu.MenuItems.AddRange((file, tools, view))
		return menu

	def _timer_Tick(sender, args as EventArgs):
		_timer.Enabled = false
		
		if File.Exists(GetDockStateXmlFileName()):
			LoadDockState()
			OpenDocuments(_argv)
		else:
			if len(_argv):			
				OpenDocuments(_argv)
			else:
				NewDocument()
			ShowDocumentOutline()

	StatusText as string:
		set:
			_statusPanel1.Text = value
			
	def ParseString(fname as string, code as string):		
		try:
			_parser.Parameters.Input.Add(StringInput(fname, code))
			return _parser.Run().CompileUnit
		ensure:
			_parser.Parameters.Input.Clear()

	private def LoadInterceptors():
		tempInterceptors = []
		
		if _settings.LoadPlugins:
			for file in Directory.GetFiles(MapPath("scripts"), "*.int"):
				interceptors = LoadInterceptorsFromFile(file)
				tempInterceptors = tempInterceptors + interceptors if interceptors

		_textInterceptors = array(ITextInterceptor, tempInterceptors)
		StatusText = "Loaded ${len(_textInterceptors)} TextInterceptor(s)"
		
	def MapPath(path as string):
		return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), path)

	def LoadInterceptorsFromFile(fileName as string):
		script = ScriptCompiler.CompileFile(fileName)
		if len(script.Errors):
			for error in script.Errors:
				print("Compiler error: ${error}")
			return null
		
		retTypes = script.GetTypes()
		return null unless retTypes
		return [cast(ITextInterceptor, retType()) for retType in retTypes if retType() isa ITextInterceptor]

			
	def Expand(fname as string, code as string):
		compiler = BooCompiler()
		compiler.Parameters.OutputWriter = StringWriter()
		compiler.Parameters.Pipeline = CompileToBoo()
		compiler.Parameters.Input.Add(StringInput(fname, code))
		
		result = compiler.Run()
		self.UpdateTaskList(result.Errors)
		
		NewDocument().TextContent = compiler.Parameters.OutputWriter.ToString()
			
			
	def ShowDocumentOutline():
		ShowContent(_documentOutline)
		
	private def ShowContent(content as DockContent):
		content.Show(_dockPanel)
		if DockState.Unknown == content.DockState:
			content.Pane.DockState = content.ShowHint

	def NewDocument():
		editor = BooEditor(self)
		editor.Show(_dockPanel)
		editor.TextArea.Focus()
		return editor
		
	def OpenDocuments([required] fnames):
		for fname in fnames:
			try:
				OpenDocument(fname)
			except x:
				print(x)
				
	def OpenDocument([required] filename as string):
		filename = Path.GetFullPath(filename)
		content = FindEditor(filename)
		if content is null:
			editor = CreateEditor(filename)
			editor.Show(_dockPanel)
			editor.TextArea.Focus()
			return editor
		else:
			content.Show(_dockPanel)
			content.TextArea.Focus()
			return content
			
	private def CreateEditor([required] fname as string):
		editor = BooEditor(self)
		editor.Open(fname)
		return editor

	def _dockPanel_ActiveDocumentChanged(sender, args as EventArgs):
		document = _dockPanel.ActiveDocument
		editor = document as BooEditor
		_documentOutline.ActiveDocument = editor
		_menuItemClose.Enabled = document is not null
		_menuItemSaveAs.Enabled = _menuItemSave.Enabled = document is not null

	def _menuItemSaveAs_Click(sender, args as EventArgs):
		cast(BooEditor, _dockPanel.ActiveDocument).SaveAs()

	def _menuItemSave_Click(sender, args as EventArgs):
		cast(BooEditor, _dockPanel.ActiveDocument).Save()

	def _menuItemExit_Click(sender, args as EventArgs):
		self.Close()

	def _menuItemClose_Click(sender, args as EventArgs):		
		_dockPanel.ActiveDocument.Close()

	def _menuItemDocumentOutline_Click(sender, args as EventArgs):
		ShowDocumentOutline()

	def _menuItemTaskList_Click(sender, args as EventArgs):
		ShowTaskList()
		
	def UpdateTaskList(errors as CompilerErrorCollection):
		_taskList.Update(errors)
		ShowTaskList() if len(errors)

	def ShowTaskList():
		ShowContent(_taskList)
		
	def ShowOutputPane():
		ShowContent(_outputPane)
		
	def ShowPrompt():
		if _interactiveConsole.IsDisposed:
			_interactiveConsole = BooExplorer.InteractiveConsole(self)
		ShowContent(_interactiveConsole)

	def _menuItemOutputPane_Click(sender, args as EventArgs):
		ShowOutputPane()
		
	def _menuItemOptions_Click():
		using dlg = Form(Text: "Options"):
			dlg.Controls.Add(PropertyGrid(
								Dock: DockStyle.Fill,
								SelectedObject: _settings,
								Font: Font,
								PropertySort: PropertySort.Alphabetical))
			dlg.ShowDialog()
			SaveSettings()

	def _menuItemOpen_Click(sender, args as EventArgs):
		using dlg = OpenFileDialog(
					Filter: "boo files (*.boo)|*.boo|All files (*.*)|*.*",
					Multiselect: true):
			if DialogResult.OK == dlg.ShowDialog(self):
				for fname in dlg.FileNames:
					OpenDocument(fname)

	def _menuItemNew_Click(sender, args as EventArgs):
		NewDocument()
		
	def GetApplicationDataFolder():
		folder = Application.UserAppDataPath				
		Directory.CreateDirectory(folder) unless Directory.Exists(folder)
		return folder
		
	def GetDockStateXmlFileName():
		return Path.Combine(GetApplicationDataFolder(), "dockstate.xml")
		
	def SaveDockState():
		_dockPanel.SaveAsXml(GetDockStateXmlFileName())
		
	def LoadDockState():		
		_dockPanel.LoadFromXml(GetDockStateXmlFileName(), OnDeserializeDockContent)
		
	def OnDeserializeDockContent(persistString as string) as DockContent:
		type, content = /\|/.Split(persistString)
		print("type: ${type}, content: ${content}")
		if "DocumentOutline" == type:
			return _documentOutline
		if "InteractiveConsole" == type:
			return _interactiveConsole
		if "TaskList" == type:
			return _taskList
		if "OutputPane" == type:
			return _outputPane
		if "BooEditor" == type:
			editor = BooEditor(self)
			editor.Open(content) if File.Exists(content)
			return editor
		raise ArgumentException("Invalid persistence string: ${persistString}")

	override protected def OnClosing(args as CancelEventArgs):
		super(args)
		if not _isQuitting and not args.Cancel:			
			dirtyDocuments = [
							editor.GetSafeFileName()
							for document in _dockPanel.Documents
							if (editor=(document as BooEditor)) and editor.IsDirty
							]
			return unless len(dirtyDocuments)
			
			args.Cancel = (
							DialogResult.Yes !=
							MessageBox.Show("The following files were modified:\n\n\t" + 
											join(dirtyDocuments, "\n\t") + 
											"\n\nAre you sure you want to leave and lose all your changes?",
											"Boo Explorer",
											MessageBoxButtons.YesNo))
		_isQuitting = not args.Cancel

	def FindEditor(fname as string):
		for document in _dockPanel.Documents:
			editor = document as BooEditor
			if editor and editor.FileName == fname:
				return editor
		return null
