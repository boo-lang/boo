#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

import System
import System.IO
import System.Drawing from System.Drawing
import System.Windows.Forms from System.Windows.Forms

def ScanFile(lv as ListView, fname as string, pattern as string):
	using stream=File.OpenText(fname):	
		for index as int, line as string in enumerate(fname):
			if line =~ pattern:
				lvItem = lv.Items.Add(fname)
				lvItem.SubItems.Add(index.ToString())
				lvItem.Tag = [fname, index]
		
def ScanDirectory(lv as ListView, path as string, glob as string, pattern as string):
	for fname in Directory.GetFiles(path, glob):
		ScanFile(lv, fname, pattern)
	for path in Directory.GetDirectories(path):
		ScanDirectory(lv, path, glob, pattern)
		
def fileList_SelectedIndexChanged(sender, args as EventArgs):	
	fileList as ListView = sender
	txtBox as TextBox = fileList.Tag
	for item as ListViewItem in fileList.SelectedItems:
		fname as string, index as int = item.Tag
		txtBox.Text = File.ReadAllText(fname)
		txtBox.Focus()
		txtBox.SelectionLength = 0		
		txtBox.SelectionStart = index		
		txtBox.ScrollToCaret()

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
txtBox = TextBox(Dock: DockStyle.Fill,
					AcceptsTab: true,
					Multiline: true,
					ScrollBars: ScrollBars.Vertical,
					Font: Font("Lucida Console", 12))
textTab.Controls.Add(txtBox)
fileTab.Controls.Add(textTab)

fileList.Tag = txtBox

f = Form(Text: "Visual Grep Utility",
			Font: Font("Tahoma", 8),
			Size: Size(800, 600))

f.Controls.Add(fileTab)
f.Controls.Add(splitter)
f.Controls.Add(fileList)

glob, pattern = argv
ScanDirectory(fileList, ".", glob, pattern)

Application.Run(f)


