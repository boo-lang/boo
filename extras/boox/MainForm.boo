namespace BooExplorer

import System
import System.ComponentModel
import System.Windows.Forms
import System.Drawing
import WeifenLuo.WinFormsUI

class MainForm(Form):	
	
	_dockManager as DockManager
	_status as StatusBar
	_statusPanel1 as StatusBarPanel
	
	[getter(DocumentOutline)]
	_classBrowser = BooExplorer.DocumentOutline()

	_menuItemClose as MenuItem
	_menuItemSave as MenuItem
	_menuItemSaveAs as MenuItem
	
	def constructor():		
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

		Controls.Add(_dockManager)
		Controls.Add(_status)
		ResumeLayout(false)
		
	private def CreateMainMenu():
		
		menu = MainMenu()
		file = MenuItem(Text: "&File")
		file.MenuItems.Add(MenuItem(Text: "&Open...",
									Click: _menuItemOpen_Click,
									Shortcut: Shortcut.CtrlO))
		file.MenuItems.Add(MenuItem(Text: "&New",
									Click: _menuItemNew_Click,
									Shortcut: Shortcut.CtrlN))
									
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
									
		file.MenuItems.Add(MenuItem(Text: "-"))
		file.MenuItems.Add(MenuItem(Text: "E&xit",
									Shortcut: Shortcut.CtrlQ,
									Click: _menuItemExit_Click))
		
		tools = MenuItem(Text: "&View")
		tools.MenuItems.Add(MenuItem(Text: "Document Outline", Click: _menuItemDocumentOutline_Click))
		
		menu.MenuItems.AddRange((file, tools))
		return menu
		
	StatusText as string:
		set:
			_statusPanel1.Text = value
		
	def NewDocument():
		editor = BooEditor(self)
		editor.Show(_dockManager)
		editor.TextArea.Focus()		
		
	def _dockManager_ActiveDocumentChanged(sender, args as EventArgs):
		document = _dockManager.ActiveDocument
		editor = document as BooEditor		
		_classBrowser.ActiveDocument = editor
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
		_classBrowser.Show(_dockManager)
		
	def _menuItemOpen_Click(sender, args as EventArgs):
		dlg = OpenFileDialog(
					Filter: "boo files (*.boo)|*.boo|All files (*.*)|*.*")
		if DialogResult.OK == dlg.ShowDialog(self):
			content = FindEditor(dlg.FileName)
			if content is null:
				editor = BooEditor(self)
				editor.Open(dlg.FileName)
				editor.Show(_dockManager)
				editor.TextArea.Focus()
			else:
				content.Show(_dockManager)
				content.TextArea.Focus()				
		
	def _menuItemNew_Click(sender, args as EventArgs):
		NewDocument()
		
	override protected def OnClosing(args as CancelEventArgs):
		super(args)
		if (not args.Cancel) and AreThereDirtyDocuments():
			args.Cancel = (
							DialogResult.Yes !=
							MessageBox.Show("Are you sure you want to leave and lose all your changes?",
											"Boo Explorer",
											MessageBoxButtons.YesNo))
			
	def AreThereDirtyDocuments():
		for editor as BooEditor in _dockManager.Documents:
			if editor.IsDirty:
				return true
		return false
		
	def FindEditor(fname as string):
		for document in _dockManager.Documents:
			editor = document as BooEditor
			if editor and editor.FileName == fname:
				return editor
		return null
