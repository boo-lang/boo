using System.Drawing from System.Drawing
using System.Windows.Forms from System.Windows.Forms

fileList = ListView(
				Dock: DockStyle.Bottom,
				TabIndex: 0,
				Size: Size(576, 144),
				View: View.Details)
fileList.Columns.Add("File", 200, HorizontalAlignment.Left)
fileList.Columns.Add("Line", 50, HorizontalAlignment.Left)

				
splitter = Splitter(Dock: DockStyle.Bottom, TabIndex: 1, TabStop: false)

fileTab = TabControl(Dock: DockStyle.Fill)
textTab = TabPage(TabIndex: 0, Text: "FileName goes here")
richText = RichTextBox(Dock: DockStyle.Fill)
textTab.Controls.Add(richText)
fileTab.Controls.Add(textTab)

f = Form(Text: "Visual Grep Utility")

f.Controls.Add(fileTab)
f.Controls.Add(splitter)
f.Controls.Add(fileList)

Application.Run(f)
