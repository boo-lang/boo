namespace BooExplorer

import System
import System.Environment
import System.IO
import System.ComponentModel
import System.Windows.Forms
import System.Drawing
import WeifenLuo.WinFormsUI

class MainForm(Form):

	_dockPanel as DockPanel
	_status as StatusBar
	_statusPanel1 as StatusBarPanel
	_timer as Timer

	[getter(DocumentOutline)]
	_documentOutline = BooExplorer.DocumentOutline()

	[getter(TaskList)]
	_taskList = BooExplorer.TaskList(self)
	
	[getter(OutputPane)]
	_outputPane = BooExplorer.OutputPane()

	_argv as (string)

	_menuItemClose as MenuItem
	_menuItemSave as MenuItem
	_menuItemSaveAs as MenuItem

	def constructor(argv as (string)):
		_argv = argv
		_dockPanel = DockPanel(Dock: DockStyle.Fill,
						ActiveAutoHideContent: null,
						TabIndex: 1,
						ActiveDocumentChanged: _dockPanel_ActiveDocumentChanged)

		_statusPanel1 = StatusBarPanel(AutoSize: StatusBarPanelAutoSize.Contents)

		_status = StatusBar(ShowPanels: true, TabIndex: 2)
		_status.Panels.Add(_statusPanel1)

		SuspendLayout()

		self.Size = System.Drawing.Size(800, 600)
		self.Menu = CreateMainMenu()
		self.Text = "Boo Explorer"
		self.IsMdiContainer = true

		Controls.AddRange((
					_dockPanel,
					_status))
		ResumeLayout(false)
		
		_timer = Timer(Tick: _timer_Tick, Interval: 50ms.TotalMilliseconds)
		_timer.Enabled = true

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
						Shortcut: Shortcut.CtrlShiftO)
			))


		menu.MenuItems.AddRange((file, view))
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

	def ShowTaskList():
		ShowContent(_taskList)
		
	def ShowOutputPane():
		ShowContent(_outputPane)

	def _menuItemOutputPane_Click(sender, args as EventArgs):
		ShowOutputPane()

	def _menuItemOpen_Click(sender, args as EventArgs):
		dlg = OpenFileDialog(
					Filter: "boo files (*.boo)|*.boo|All files (*.*)|*.*",
					Multiselect: true)
		if DialogResult.OK == dlg.ShowDialog(self):
			for fname in dlg.FileNames:
				OpenDocument(fname)

	def _menuItemNew_Click(sender, args as EventArgs):
		NewDocument()
		
	def GetApplicationDataFolder():
		folder = Path.Combine(
				GetFolderPath(SpecialFolder.ApplicationData),
				"boox")
				
		Directory.CreateDirectory(folder) unless Directory.Exists(folder)
		return folder
		
	def GetDockStateXmlFileName():
		return Path.Combine(GetApplicationDataFolder(), "dockstate.xml")
		
	def SaveDockState():
		_dockPanel.SaveAsXml(GetDockStateXmlFileName())
		
	def LoadDockState():
		delegate = Delegate.CreateDelegate(DeserializeDockContent, self, "OnDeserializeDockContent")
		_dockPanel.LoadFromXml(GetDockStateXmlFileName(), delegate)
		
	def OnDeserializeDockContent(persistString as string) as DockContent:
		type, content = /\|/.Split(persistString)
		print("type: ${type}, content: ${content}")
		if "DocumentOutline" == type:
			return _documentOutline
		if "TaskList" == type:
			return _taskList
		if "OutputPane" == type:
			return _outputPane
		if "BooEditor" == type:
			editor = BooEditor(self)
			editor.Open(content) if File.Exists(content)
			return editor
		raise ArgumentException("Invalid persistence string: ${persistString}")
		
	override protected def OnClosed(args as EventArgs):
		super(args)
		SaveDockState()

	override protected def OnClosing(args as CancelEventArgs):
		super(args)
		if not args.Cancel:			
			dirtyDocuments = [
							cast(BooEditor, editor).GetSafeFileName()
							for editor in _dockPanel.Documents
							if editor isa BooEditor and cast(BooEditor, editor).IsDirty
							]
			return unless len(dirtyDocuments)
			
			args.Cancel = (
							DialogResult.Yes !=
							MessageBox.Show("The following files were modified:\n\n\t" + 
											join(dirtyDocuments, "\n\t") + 
											"\n\nAre you sure you want to leave and lose all your changes?",
											"Boo Explorer",
											MessageBoxButtons.YesNo))

	def FindEditor(fname as string):
		for document in _dockPanel.Documents:
			editor = document as BooEditor
			if editor and editor.FileName == fname:
				return editor
		return null
