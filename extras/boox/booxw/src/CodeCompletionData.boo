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

import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
import ICSharpCode.TextEditor.Actions
import ICSharpCode.TextEditor.Gui.CompletionWindow

import System
import BooExplorer.Common
import Boo.Lang.Compiler.TypeSystem

class CodeCompletionData(ICompletionData, IComparable):
	_description as string

	Description as string:
		get:
			if not _overloads:
				return _description
			else:
				return "${_description} (+${_overloads} overloads)"
		set:
			_description = value

	[getter(ImageIndex)]
	_imageIndex as int = 0

	[getter(Text)]
	_text as (string)

	[property(Overloads)]
	_overloads as int

	def constructor(imageIndex as int, [required]text as string, [required]description as string):
		_imageIndex = imageIndex
		_text = (text,)
		_description = description
	
	def InsertAction(control as TextEditorControl):
		control.ActiveTextAreaControl.TextArea.InsertString(_text[0])

	public def CompareTo(obj) as int:
		if obj is null or not obj isa CodeCompletionData:
			return -1

		temp = obj as CodeCompletionData
		return _text[0].CompareTo(temp.Text[0])

class CodeCompletionDataProvider(ICompletionDataProvider):

	_codeCompletion as (IEntity)

	ImageList as System.Windows.Forms.ImageList:
		get:
			return BooxImageList.Instance
			
	PreSelection as string:
		get:
			return null
	
	def constructor(codeCompletion):
		_codeCompletion = codeCompletion

	def GenerateCompletionData(fileName as string, textArea as TextArea, charTyped as System.Char) as (ICompletionData):
		values = {}
		for item in _codeCompletion:
			continue if item.Name[:4] in ("add_", "remove_", "get_", "set_")
			if not "." in item.Name:
				if not values[item.Name]:
					values[item.Name] = CodeCompletionData(GetImageIndex(item), item.Name, item.ToString())
				else:
					++(values[item.Name] as CodeCompletionData).Overloads
		return array(ICompletionData, values.Values)
		
	def GetImageIndex(entity as IEntity):
		type = entity.EntityType
		if EntityType.Property == type:
			p as IProperty = entity
			return cast(int, TypeIcon.PublicProperty) if p.IsPublic
			return cast(int, TypeIcon.PrivateProperty)		
		if EntityType.Field == type:
			f as IField = entity
			return cast(int, TypeIcon.PublicField) if f.IsPublic
			return cast(int, TypeIcon.PrivateField)
		if EntityType.Event == type:
			return cast(int, TypeIcon.PublicEvent)
		if type in (EntityType.Method, EntityType.Constructor):
			m as IMethod = entity
			return cast(int, TypeIcon.PublicMethod) if m.IsPublic
			return cast(int, TypeIcon.PrivateMethod)
		return 0
	
