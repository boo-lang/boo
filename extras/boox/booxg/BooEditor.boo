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

import System
import System.Resources
import System.IO
import Boo.Lang.Useful.IO
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Ast
import Gtk
import GtkSourceView

class BooEditor(ScrolledWindow):
	
	[getter(FileName)]
	_fname as string
	
	_view = BooSourceView()
	
	[getter(Buffer)]
	_buffer = _view.SourceBuffer
	
	event LabelChanged as EventHandler
	
	Label:
		get:
			suffix = " *" if _buffer.Modified
			return System.IO.Path.GetFileName(_fname) + suffix if _fname
			return "unnamed.boo" + suffix
	
	def constructor():
		self.SetPolicy(PolicyType.Automatic, PolicyType.Automatic)
		self.Add(_view)
		_buffer.ModifiedChanged += { LabelChanged(self, EventArgs.Empty) }
		
	def constructor(ptr as System.IntPtr):
		super(ptr)
		
	def Open([required] fname as string):
		fname = System.IO.Path.GetFullPath(fname)
		_buffer.Text = File.ReadAllText(fname)
		_buffer.Modified = false
		_fname = fname
		
	def SaveAs([required] fname as string):
		File.WriteAllText(fname, _buffer.Text)
		_fname = fname
		_buffer.Modified = false
			
	def Redo():
		_buffer.Redo()
		
	def Undo():
		_buffer.Undo()

