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

namespace booish.gui

import System
import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
import ICSharpCode.TextEditor.Actions
import ICSharpCode.TextEditor.Gui.CompletionWindow
import Boo.Lang.Compiler.TypeSystem

internal class CodeCompletionData(ICompletionData, IComparable):
	
	_name as string
	_imageProvider as ICompletionWindowImageProvider
	_entities as List = []
	
	def constructor(imageProvider, name):
		_imageProvider = imageProvider
		_name = name
	
	Text as (string):
		get:
			return (_name,)
			
	Description:
		get:
			return "${cast(IEntity, _entities[0]).FullName} (${len(_entities)} overloads)"
			
	ImageIndex as int:
		get:
			return _imageProvider.GetImageIndex(_entities[0])
		
	def AddEntity(entity as IEntity):
		_entities.Add(entity)
	
	def InsertAction(control as TextEditorControl):
		control.ActiveTextAreaControl.TextArea.InsertString(Text[0])

	public def CompareTo(obj) as int:
		if obj is null or not obj isa CodeCompletionData:
			return -1

		other = obj as CodeCompletionData
		return _name.CompareTo(other._name)

internal class CodeCompletionDataProvider(ICompletionDataProvider):

	_imageProvider as ICompletionWindowImageProvider
	_codeCompletion as (IEntity)

	ImageList as System.Windows.Forms.ImageList:
		get:
			return _imageProvider.ImageList
			
	PreSelection as string:
		get:
			return null
	
	def constructor(imageProvider, codeCompletion):
		_imageProvider = imageProvider
		_codeCompletion = codeCompletion

	def GenerateCompletionData(fileName as string, textArea as TextArea, charTyped as System.Char) as (ICompletionData):		
		values = {}
		for item in _codeCompletion:			
			continue if IsSpecial(item)
			
			data as CodeCompletionData
			data = values[item.Name]
			if data is null:
				name = item.Name
				if "." in name:
					name = /\./.Split(name)[-1]			
				data = CodeCompletionData(_imageProvider, name)
				values[item.Name] = data
			data.AddEntity(item)
		return array(ICompletionData, values.Values)
		
	def IsSpecial(entity as IEntity):
		for prefix in ".", "___", "add_", "remove_", "get_", "set_":
			return true if entity.Name.StartsWith(prefix)
		
	def GetImageIndex(entity as IEntity):		
		return 0
	
