using System.IO
using Boo.IO
using System.Drawing from System.Drawing
using System.Windows.Forms from System.Windows.Forms

def ScanFile(lv as ListView, fname as string, pattern as string):	
	for index, line as string in enumerate(TextFile(fname)):
		lv.Items.Add(fname).SubItems.Add(index.ToString()) if line =~ pattern
		
def ScanDirectory(lv as ListView, path as string, glob as string, pattern as string):
	for fname in Directory.GetFiles(path, glob):
		ScanFile(lv, fname, pattern)
	for path in Directory.GetDirectories(path):
		ScanDirectory(lv, path, glob, pattern)
		
def fileList_SelectedIndexChanged(sender, args as EventArgs):
	MessageBox.Show("ok!")
	#fileList as ListView = sender
	#richText as RichTextBox = fileList.Tag
	#richText.LoadFile()

fileList = ListView(
				Dock: DockStyle.Bottom,
				TabIndex: 0,
				Size: Size(576, 144),
				View: View.Details,
				FullRowSelect: true,
				SelectedIndexChanged: fileList_SelectedIndexChanged	)
fileList.Columns.Add("File", 400, HorizontalAlignment.Left)
fileList.Columns.Add("Line", 50, HorizontalAlignment.Left)

				
splitter = Splitter(Dock: DockStyle.Bottom, TabIndex: 1, TabStop: false)

fileTab = TabControl(Dock: DockStyle.Fill)
textTab = TabPage(TabIndex: 0, Text: "FileName goes here")
richText = RichTextBox(Dock: DockStyle.Fill, AcceptsTab: true, Font: Font("Lucida Console", 12))
textTab.Controls.Add(richText)
fileTab.Controls.Add(textTab)

f = Form(Text: "Visual Grep Utility",
			Font: Font("Tahoma", 8),
			Size: Size(800, 600))

f.Controls.Add(fileTab)
f.Controls.Add(splitter)
f.Controls.Add(fileList)

_, glob, pattern = System.Environment.GetCommandLineArgs()
ScanDirectory(fileList, ".", glob, pattern)

Application.Run(f)


