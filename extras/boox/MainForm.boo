namespace BooExplorer

import System
import System.Windows.Forms
import System.Drawing
import WeifenLuo.WinFormsUI

class MainForm(Form):	
	
	_dockManager as DockManager
	_status as StatusBar
	
	[getter(ClassBrowser)]
	_classBrowser = BooExplorer.ClassBrowser()
	
	def constructor():		
		_dockManager = DockManager(Dock: DockStyle.Fill,
						ActiveAutoHideContent: null,
						TabIndex: 1,
						ActiveDocumentChanged: _dockManager_ActiveDocumentChanged)
						
		_status = StatusBar(ShowPanels: true, TabIndex: 2)

		SuspendLayout()
		
		self.Menu = CreateMainMenu()
		self.Text = "Boo Explorer"		
		self.IsMdiContainer = true
		self.WindowState = FormWindowState.Maximized

		Controls.Add(_dockManager)
		Controls.Add(_status)
		ResumeLayout(false)
		
	private def CreateMainMenu():
		
		menu = MainMenu()
		file = MenuItem(Text: "&File")
		file.MenuItems.Add(MenuItem(Text: "&Open...", Click: _menuItemOpen_Click))
		file.MenuItems.Add(MenuItem(Text: "&New", Click: _menuItemNew_Click))
		
		tools = MenuItem(Text: "&Tools")
		tools.MenuItems.Add(MenuItem(Text: "Class Browser", Click: _menuItemClassBrowser_Click))
		
		menu.MenuItems.AddRange((file, tools))
		return menu
		
	def NewDocument():
		BooEditor(self).Show(_dockManager)
		
	def _dockManager_ActiveDocumentChanged(sender, args as EventArgs):
		editor = _dockManager.ActiveDocument as BooEditor		
		_classBrowser.ActiveDocument = editor
		
	def _menuItemClassBrowser_Click(sender, args as EventArgs):
		_classBrowser.Show(_dockManager)
		
	def _menuItemOpen_Click(sender, args as EventArgs):
		dlg = OpenFileDialog(
					Filter: "boo files (*.boo)|*.boo|All files (*.*)|*.*")
		if DialogResult.OK == dlg.ShowDialog(self):
			content = FindDocument(dlg.FileName)
			if content is null:
				editor = BooEditor(self)
				editor.LoadFile(dlg.FileName)
				editor.Show(_dockManager)
			else:
				content.Show(_dockManager)
		
	def _menuItemNew_Click(sender, args as EventArgs):
		NewDocument()
		
	def FindDocument(text as string):
		for document in _dockManager.Documents:
			if document.Text == text:
				return document
		return null
