import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
import ICSharpCode.TextEditor.Actions
import WeifenLuo.WinFormsUI
import System
import System.Windows.Forms
import System.Drawing
import System.IO
import System.Threading

class BooFormattingStrategy(DefaultFormattingStrategy):
	override def SmartIndentLine(area as TextArea, line as int) as int:
		document = area.Document
		previousLine = document.GetLineSegment(line-1)
		
		if document.GetText(previousLine).EndsWith(":"):
			currentLine = document.GetLineSegment(line)
			indentation = GetIndentation(area, line-1)
			indentation += Tab.GetIndentationString(document)
			document.Replace(currentLine.Offset,
							currentLine.Length,
							indentation+document.GetText(currentLine))
			return len(indentation)
		
		return super(area, line)
		
class FileEditor(Content):
	
	_editor as TextEditorControl
	
	def constructor():		
		_editor = TextEditorControl(Dock: DockStyle.Fill,
							Font: System.Drawing.Font("Lucida Console", 13.0),
							EnableFolding: true)

		_editor.Document.FormattingStrategy = BooFormattingStrategy()
		_editor.Document.HighlightingStrategy = GetBooHighlighting()
		
		SuspendLayout()
		Controls.Add(_editor)
		self.Text = "boo"		
		self.DockPadding.All = 1
		ResumeLayout(false)
		
	def LoadFile(fname as string):
		self.Text = fname
		_editor.LoadFile(fname)
	
	def GetBooHighlighting():
		return HighlightingManager.Manager.FindHighlighter("boo")
		
class MainForm(Form):
	
	_dockManager as DockManager
	_status as StatusBar
	
	def constructor():		
		_dockManager = DockManager(Dock: DockStyle.Fill,
						ActiveAutoHideContent: null,
						TabIndex: 1)
						
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
		file.MenuItems.Add(MenuItem(Text: "&Open...", Click: _open_Click))
		file.MenuItems.Add(MenuItem(Text: "&New", Click: _new_Click))
		
		menu.MenuItems.AddRange((file,))
		return menu
		
	def _open_Click(sender, args as EventArgs):
		dlg = OpenFileDialog(
					Filter: "boo files (*.boo)|*.boo|All files (*.*)|*.*")
		if DialogResult.OK == dlg.ShowDialog(self):
			content = FindDocument(dlg.FileName)
			if content is null:
				editor = FileEditor()
				editor.LoadFile(dlg.FileName)
				editor.Show(_dockManager)
			else:
				content.Show(_dockManager)
		
	def _new_Click(sender, args as EventArgs):
		FileEditor().Show(_dockManager)
		
	def FindDocument(text as string):
		for document in _dockManager.Documents:
			if document.Text == text:
				return document
		return null

	
def GetAssemblyFolder():
	return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)

Thread.CurrentThread.ApartmentState = ApartmentState.STA

HighlightingManager.Manager.AddSyntaxModeFileProvider(
		FileSyntaxModeProvider(GetAssemblyFolder()))
/*
for fname in argv:
	editor = FileEditor()
	editor.LoadFile(Path.GetFullPath(fname))
	editor.Show(dockManager)*/

Application.Run(MainForm())
