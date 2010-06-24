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

class MainForm(Form):
	
	_fileList as ListView
	_filesTab as TabControl
	_editor as TextBox	
	_splitter as Splitter
	
	def constructor(glob as string, pattern as string):
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
							ScrollBars: ScrollBars.Vertical | ScrollBars.Horizontal,
							Font: System.Drawing.Font("Lucida Console", 12))
							
		editorTab = TabPage(TabIndex: 0, Text: "FileName goes here")
		editorTab.Controls.Add(_editor)
							
		_filesTab = TabControl(Dock: DockStyle.Fill)		
		_filesTab.Controls.Add(editorTab)
		
		Controls.Add(_filesTab)
		Controls.Add(_splitter)
		Controls.Add(_fileList)
		
		ScanDirectory(".", glob, pattern)

	def ScanFile(fname as string, pattern as string):
		position = 0
		newLineLen = len(Environment.NewLine)
		using stream=File.OpenText(fname):
			for index, line as string in enumerate(stream):
				if line =~ pattern:
					lvItem = _fileList.Items.Add(fname)
					lvItem.SubItems.Add(index.ToString())
					lvItem.Tag = (fname, position)
				position = position + len(line) + newLineLen
			
	def ScanDirectory(path as string, glob as string, pattern as string):
		for fname in Directory.GetFiles(path, glob):
			ScanFile(fname, pattern)
		for path in Directory.GetDirectories(path):
			ScanDirectory(path, glob, pattern)
			
	def _fileList_SelectedIndexChanged(sender, args as EventArgs):
		
		for lvItem as ListViewItem in _fileList.SelectedItems:
			fname as string, position as int = lvItem.Tag
			
			_editor.Text = File.ReadAllText(fname)
			_editor.Focus()
			_editor.SelectionLength = 0		
			_editor.SelectionStart = position
			_editor.SelectionLength = 10
			_editor.ScrollToCaret()

glob, pattern = argv
Application.Run(MainForm(
					glob,
					pattern,
					Text: "Visual Grep Utility",
					Font: Font("Tahoma", 8),
					Size: Size(800, 600)))


