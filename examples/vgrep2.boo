using Boo.IO
using System
using System.IO
using System.Drawing from System.Drawing
using System.Windows.Forms from System.Windows.Forms

class MainForm(Form):
	
	_fileList as ListView
	_filesTab as TabControl
	_editor as TextBox	
	_splitter as Splitter
	
	def constructor():
		_fileList = ListView(
				Dock: DockStyle.Bottom,
				TabIndex: 0,
				Size: System.Drawing.Size(576, 144),
				View: View.Details,
				FullRowSelect: true,
				SelectedIndexChanged: _fileList_SelectedIndexChanged)
				
		_fileList.Columns.Add("File", 400, HorizontalAlignment.Left)
		_fileList.Columns.Add("Line", 50, HorizontalAlignment.Left)
		
						
		_splitter = Splitter(Dock: DockStyle.Bottom, TabStop: false)		
		
		_editor = TextBox(Dock: DockStyle.Fill,
							AcceptsTab: true,
							Multiline: true,
							ScrollBars: ScrollBars.Vertical,
							Font: System.Drawing.Font("Lucida Console", 12))
							
		editorTab = TabPage(TabIndex: 0, Text: "FileName goes here")
		editorTab.Controls.Add(_editor)
							
		_filesTab = TabControl(Dock: DockStyle.Fill)		
		_filesTab.Controls.Add(editorTab)
		
		Controls.Add(_filesTab)
		Controls.Add(_splitter)
		Controls.Add(_fileList)
		
		_, glob, pattern = System.Environment.GetCommandLineArgs()
		ScanDirectory(".", glob, pattern)

	def ScanFile(fname as string, pattern as string):
		position = 0
		for index, line as string in enumerate(TextFile(fname)):
			if line =~ pattern:
				lvItem = _fileList.Items.Add(fname)
				lvItem.SubItems.Add(index.ToString())
				lvItem.Tag = (fname, position)
				//position += line.Length
			
	def ScanDirectory(path as string, glob as string, pattern as string):
		for fname in Directory.GetFiles(path, glob):
			ScanFile(fname, pattern)
		for path in Directory.GetDirectories(path):
			ScanDirectory(path, glob, pattern)
			
	def _fileList_SelectedIndexChanged(sender, args as EventArgs):
		
		for lvItem as ListViewItem in _fileList.SelectedItems:
			fname as string, position as int = lvItem.Tag
			
			_editor.Text = TextFile.ReadFile(fname)
			_editor.Focus()
			_editor.SelectionLength = 0		
			_editor.SelectionStart = position		
			_editor.ScrollToCaret()

Application.Run(MainForm(Text: "Visual Grep Utility",
					Font: Font("Tahoma", 8),
					Size: Size(800, 600)))


