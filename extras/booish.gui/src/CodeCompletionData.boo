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
import Boo.Lang.Interpreter
import Boo.Lang.Compiler.TypeSystem

internal class AbstractCompletionData(ICompletionData, IComparable):
	
	_name as string
	
	def constructor(name):
		_name = name
		
	Text as (string):
		get:
			return (_name,)
			
	abstract ImageIndex as int:
		get:
			pass
		
	abstract Description as string:
		get:
			pass
			
	def InsertAction(control as TextEditorControl):
		control.ActiveTextAreaControl.TextArea.InsertString(_name)

	public def CompareTo(obj) as int:
		if obj is null or not obj isa CodeCompletionData:
			return -1

		other = obj as CodeCompletionData
		return _name.CompareTo(other._name)

internal class CodeCompletionData(AbstractCompletionData):
	
	_imageProvider as ICompletionWindowImageProvider
	_entities as List = []
	
	def constructor(imageProvider, name):
		super(name)
		_imageProvider = imageProvider
			
	Description:
		get:
			description = InteractiveInterpreter.DescribeEntity(_entities[0])
			return description if 1 == len(_entities)
			return "${description} (+${len(_entities)-1} overloads)"
			
	ImageIndex as int:
		get:
			entity = _entities[0] as IEntity
			entityType = entity.EntityType
			if EntityType.Type == entityType:
				type as IType = entity
				if type.IsInterface:
					return _imageProvider.InterfaceIndex
				elif type.IsEnum:
					return _imageProvider.EnumIndex
				elif type.IsValueType:
					return _imageProvider.StructIndex
				elif type isa ICallableType:
					return _imageProvider.CallableIndex
				else:
					return _imageProvider.ClassIndex
			elif EntityType.Method == entityType:
				return _imageProvider.MethodIndex
			elif EntityType.Field == entityType:
				if (entity as IField).IsLiteral:
					return _imageProvider.LiteralIndex
				else:
					return _imageProvider.FieldIndex
			elif EntityType.Property == entityType:
				return _imageProvider.PropertyIndex
			elif EntityType.Event == entityType:
				return _imageProvider.EventIndex
			return _imageProvider.NamespaceIndex
		
	def AddEntity(entity as IEntity):
		_entities.Add(entity)
		
abstract internal class AbstractCompletionDataProvider(ICompletionDataProvider):

	_imageProvider as ICompletionWindowImageProvider
	
	def constructor([required] imageProvider):
		_imageProvider = imageProvider

	ImageList as System.Windows.Forms.ImageList:
		get:
			return _imageProvider.ImageList
			
	PreSelection as string:
		get:
			return null
			
	abstract def GenerateCompletionData(fileName as string, textArea as TextArea, charTyped as System.Char) as (ICompletionData):
		pass
		
internal class GlobalsCompletionDataProvider(AbstractCompletionDataProvider):
	_interpreter as InteractiveInterpreter
	
	class GlobalCompletionData(AbstractCompletionData):
		
		[getter(ImageIndex)]
		_imageIndex as int
		
		[getter(Description)]
		_description as string
		
		def constructor(name, imageIndex, description):
			super(name)
			_imageIndex = imageIndex
			_description = description
	
	def constructor(imageProvider, interpreter):
		super(imageProvider)
		_interpreter = interpreter
		
	override def GenerateCompletionData(fileName as string, textArea as TextArea, charTyped as System.Char) as (ICompletionData):
		globals = _interpreter.globals()
		data = array(ICompletionData, len(globals))
		for index, key in enumerate(globals):
			value = _interpreter.GetValue(key)
			delegate = value as System.Delegate
			if delegate is null:				
				if value is not null:
					description = "${key} as ${InteractiveInterpreter.GetBooTypeName(value.GetType())}"
				else:
					description = "null"				
				item = GlobalCompletionData(key, _imageProvider.FieldIndex, description)
			else:
				item = GlobalCompletionData(key, _imageProvider.MethodIndex, InteractiveInterpreter.DescribeMethod(delegate.Method))
			data[index] = item
		return data
			

internal class CodeCompletionDataProvider(AbstractCompletionDataProvider):

	_codeCompletion as (IEntity)
	
	def constructor(imageProvider, [required] codeCompletion):
		super(imageProvider)
		_codeCompletion = codeCompletion

	override def GenerateCompletionData(fileName as string, textArea as TextArea, charTyped as System.Char) as (ICompletionData):		
		values = {}
		for item in _codeCompletion:
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
	
