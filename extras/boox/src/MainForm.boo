namespace BooExplorer

import System
import System.IO
import System.ComponentModel
import System.Windows.Forms
import System.Drawing
import WeifenLuo.WinFormsUI

class MainForm(Form):

	_dockManager as DockManager
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
		_dockManager = DockManager(Dock: DockStyle.Fill,
						ActiveAutoHideContent: null,
						TabIndex: 1,
						ActiveDocumentChanged: _dockManager_ActiveDocumentChanged,
						ContentAdded: _dockManager_ContentAdded,
						ContentRemoved: _dockManager_ContentRemoved)

		_statusPanel1 = StatusBarPanel(AutoSize: StatusBarPanelAutoSize.Contents)

		_status = StatusBar(ShowPanels: true, TabIndex: 2)
		_status.Panels.Add(_statusPanel1)

		SuspendLayout()

		self.Size = System.Drawing.Size(800, 600)
		self.Menu = CreateMainMenu()
		self.Text = "Boo Explorer"
		self.IsMdiContainer = true

		Controls.AddRange((
					_dockManager,
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

		NewDocument() unless len(_argv)
		
		for fname in _argv:
			try:
				OpenDocument(fname)
			except x:
				print(x)
				
		ShowDocumentOutline()

	StatusText as string:
		set:
			_statusPanel1.Text = value
			
	def ShowDocumentOutline():
		_documentOutline.Show(_dockManager)

	def NewDocument():
		editor = BooEditor(self)
		editor.Show(_dockManager)
		editor.TextArea.Focus()

	def OpenDocument([required] filename as string):
		filename = Path.GetFullPath(filename)
		content = FindEditor(filename)
		if content is null:
			editor = BooEditor(self)
			editor.Open(filename)
			editor.Show(_dockManager)
			editor.TextArea.Focus()
			return editor
		else:
			content.Show(_dockManager)
			content.TextArea.Focus()
			return content


	def _dockManager_ActiveDocumentChanged(sender, args as EventArgs):
		document = _dockManager.ActiveDocument
		editor = document as BooEditor
		_documentOutline.ActiveDocument = editor
		_menuItemClose.Enabled = document is not null
		_menuItemSaveAs.Enabled = _menuItemSave.Enabled = document is not null

	def _dockManager_ContentAdded(sender, args as ContentEventArgs):
		pass

	def _dockManager_ContentRemoved(sender, args as ContentEventArgs):
		pass

	def _menuItemSaveAs_Click(sender, args as EventArgs):
		cast(BooEditor, _dockManager.ActiveDocument).SaveAs()

	def _menuItemSave_Click(sender, args as EventArgs):
		cast(BooEditor, _dockManager.ActiveDocument).Save()

	def _menuItemExit_Click(sender, args as EventArgs):
		self.Close()

	def _menuItemClose_Click(sender, args as EventArgs):
		_dockManager.ActiveDocument.Close()

	def _menuItemDocumentOutline_Click(sender, args as EventArgs):
		ShowDocumentOutline()

	def _menuItemTaskList_Click(sender, args as EventArgs):
		ShowTaskList()

	def ShowTaskList():
		_taskList.Show(_dockManager)
		
	def ShowOutputPane():
		_outputPane.Show(_dockManager)

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

	override protected def OnClosing(args as CancelEventArgs):
		super(args)
		if not args.Cancel:			
			dirtyDocuments = [
							editor.GetSafeFileName()
							for editor as BooEditor in _dockManager.Documents
							if editor.IsDirty
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
		for document in _dockManager.Documents:
			editor = document as BooEditor
			if editor and editor.FileName == fname:
				return editor
		return null
