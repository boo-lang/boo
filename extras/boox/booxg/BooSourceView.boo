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

class BooSourceView(SourceView):

	static _booSourceLanguage = SourceLanguagesManager().GetLanguageFromMimeType("text/x-boo")
	
	[getter(SourceBuffer)]
	_buffer as SourceBuffer
	
	def constructor():
		super(_buffer = GtkSourceView.SourceBuffer(
							_booSourceLanguage,
							Highlight: true))
		self.ShowLineNumbers = true
		self.AutoIndent = false
		self.TabsWidth = 4
		font = (
				Pango.FontDescription.FromString("Lucida Console, 12") or
				Pango.FontDescription.FromString("monospaced, 12"))
		self.ModifyFont(font)
		
	override def OnKeyPressEvent(ev as Gdk.EventKey):
		if Gdk.Key.Return == ev.Key:
			iter = _buffer.GetIterAtMark(_buffer.InsertMark)
			if iter.BackwardChar():
				line = GetLine(iter.Line)
				indent = /^(\s*)/.Match(line).Groups[0].Value
				if iter.Char == ":":
					indent += "\t"
				_buffer.InsertAtCursor("\n${indent}")
				return true
		return super(ev)
		
	def GetLine(line as int):
		start = _buffer.GetIterAtLine(line)
		end = start.Copy()
		end.ForwardLine()
		return _buffer.GetText(start, end, false)
