#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// This file is part of Boo Explorer.
//
// Boo Explorer is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// Boo Explorer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

namespace BooExplorer

import WeifenLuo.WinFormsUI
import System
import System.IO
import System.Windows.Forms
import System.Drawing
import Boo.Lang.Compiler

class TaskList(DockContent):
	
	_list as ListView
	_main as MainForm
	
	def constructor(main as MainForm):
		_main = main
		_list = ListView(Dock: DockStyle.Fill,
						View: View.Details,
						FullRowSelect: true,
						GridLines: true,
						Click: _list_Click)
					 
		_list.Columns.Add("line"       , 50, HorizontalAlignment.Left)
		_list.Columns.Add("column"     , 50, HorizontalAlignment.Left)
		_list.Columns.Add("code"       , 75, HorizontalAlignment.Left)
		_list.Columns.Add("description", 500, HorizontalAlignment.Left)
		_list.Columns.Add("module"     , 150, HorizontalAlignment.Left)

		SuspendLayout()
		
		Controls.Add(_list)
		self.HideOnClose = true
		self.AllowRedocking = true
		self.DockableAreas = (
					DockAreas.Float |
					DockAreas.DockBottom |
					DockAreas.DockTop |
					DockAreas.DockLeft |
					DockAreas.DockRight)
		self.ClientSize = System.Drawing.Size(295, 347)
		self.ShowHint = DockState.DockBottom
		self.Text = "Task List"

		ResumeLayout(false)
		
	def Clear():
		_list.Items.Clear()
		
	def Update(errors as CompilerErrorCollection):
		_list.BeginUpdate()
		_list.Items.Clear()
		try:
			for error in errors:
				self.AddCompilerError(error)
		ensure:
			_list.EndUpdate()
		
	def AddCompilerError(error as CompilerError):
		item = _list.Items.Add(error.LexicalInfo.Line.ToString())
		item.SubItems.AddRange((
				error.LexicalInfo.Column.ToString(),
				error.Code,
				error.Message,
				error.LexicalInfo.FileName))
		item.Tag = error
		
	def _list_Click(sender, args as EventArgs):
		selected = _list.SelectedItems
		return unless len(selected) > 0
		
		error as CompilerError = selected[0].Tag
		fname = error.LexicalInfo.FileName
		if File.Exists(fname):
			document as BooEditor = _main.OpenDocument(fname)
			document.GoTo(error.LexicalInfo.Line-1)
		
	override protected def GetPersistString():
		return "TaskList|"
