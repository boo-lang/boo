namespace BooExplorer

import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
import ICSharpCode.TextEditor.Actions
import WeifenLuo.WinFormsUI
import System
import System.Windows.Forms
import System.Drawing

class BooEditor(Content):
	
	_editor as TextEditorControl
	_main as MainForm
	
	def constructor(main as MainForm):
		_main = main		
		_editor = TextEditorControl(Dock: DockStyle.Fill,
							Font: System.Drawing.Font("Lucida Console", 13.0),
							EnableFolding: true,
							Changed: _editor_Changed)

		_editor.Document.FormattingStrategy = BooFormattingStrategy()
		_editor.Document.HighlightingStrategy = GetBooHighlighting()
		
		SuspendLayout()
		Controls.Add(_editor)
		self.AllowedStates = ContentStates.Document
		self.Text = "unnamed.boo"		
		self.DockPadding.All = 1
		ResumeLayout(false)
		
	TextContent:
		get:
			return _editor.Document.TextContent
		
	def LoadFile(fname as string):
		self.Text = fname
		_editor.LoadFile(fname)
	
	def GetBooHighlighting():
		return HighlightingManager.Manager.FindHighlighter("boo")
		
	def _editor_Changed(sender, args as EventArgs):
		print('changed.')
