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
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Ast
import Gtk

class DocumentOutlineProcessor:

	_store = TreeStore(Gdk.Pixbuf, string)
	_documentOutline as TreeView
	_module as Boo.Lang.Compiler.Ast.Module
	
	static def SetUp(tree as TreeView):
		
		nameColumn = TreeViewColumn()
		pixbufRender = CellRendererPixbuf()
		nameColumn.PackStart(pixbufRender, false);
		nameColumn.AddAttribute(pixbufRender, "pixbuf", 0)
			
		labelRender = Gtk.CellRendererText()
		nameColumn.PackStart(labelRender, false);
		nameColumn.AddAttribute(labelRender, "text", 1)
		tree.AppendColumn(nameColumn)
	
	def constructor(documentOutline, editor as BooEditor):
		_module = Parse(editor.Label, editor.Buffer.Text)
		_documentOutline = documentOutline
		
	def Parse(fname, text):
		compiler = BooCompiler()
		compiler.Parameters.Input.Add(StringInput(fname, text))
		compiler.Parameters.Pipeline = Pipelines.Parse()
		return compiler.Run().CompileUnit.Modules[0]
		
	def Update():
		for type in _module.Members:
			iter = _store.AppendValues((GetIcon(type), type.Name))
			if type isa TypeDefinition:
				UpdateType(iter, type)
		_documentOutline.Model = _store
		_documentOutline.ExpandAll()
				
	def UpdateType(parent, type as TypeDefinition):
		for member in type.Members:
			iter = _store.AppendValues(parent, (GetIcon(member), member.Name))
			if member isa TypeDefinition:
				UpdateType(iter, member)
			
	def GetIcon(member as TypeMember):
		type = member.NodeType
		return ApplicationResources.Icons.Class if type == NodeType.ClassDefinition
		return ApplicationResources.Icons.Field if type == NodeType.Field
		return ApplicationResources.Icons.Event if type == NodeType.Event
		return ApplicationResources.Icons.Property if type == NodeType.Property
		return ApplicationResources.Icons.Method
	
