namespace BooExplorer

import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
import ICSharpCode.TextEditor.Actions
import ICSharpCode.TextEditor.Gui.CompletionWindow

import System

import BooExplorer.Common

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

	def constructor([required]text as string, [required]description as string):
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

	_codeCompletion as CodeCompletion

	ImageList as System.Windows.Forms.ImageList:
		get:
			return System.Windows.Forms.ImageList()
			
	PreSelection as string:
		get:
			return null
	
	def constructor(codeCompletion as CodeCompletion):
		_codeCompletion = codeCompletion

	def GenerateCompletionData(fileName as string, textArea as TextArea, charTyped as System.Char) as (ICompletionData):
		values = {}
		for item in _codeCompletion.Members:
			if not "." in item.Name:
				if not values[item.Name]:
					values[item.Name] = CodeCompletionData(item.Name, item.ToString())
				else:
					++(values[item.Name] as CodeCompletionData).Overloads
		return array(ICompletionData, values.Values)
