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

import System
import System.IO
import Boo.Lang.Useful.IO
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Ast
import Gdk as Gdk
import Gtk
import GtkSourceView
import BooExplorer.Common

class BooEditor(ScrolledWindow):

	static _booSourceLanguage = SourceLanguagesManager().GetLanguageFromMimeType("text/x-boo")
	
	[getter(Buffer)]
	_buffer = SourceBuffer(_booSourceLanguage,
							Highlight: true)
							
	_view = SourceView(_buffer,
						ShowLineNumbers: true,
						AutoIndent: true,
						TabsWidth: 4)
	
	[getter(FileName)]
	_fname as string
	
	Label:
		get:
			return System.IO.Path.GetFileName(_fname) if _fname
			return "unnamed.boo"
	
	def constructor():
		self.SetPolicy(PolicyType.Automatic, PolicyType.Automatic)
		self.Add(_view)	
		
	def Open([required] fname as string):
		_buffer.Text = TextFile.ReadFile(fname)
		_buffer.Modified = false
		_fname = fname
		
	def SaveAs([required] fname as string):
		TextFile.WriteFile(fname, _buffer.Text)
		_fname = fname
		_buffer.Modified = false
			
	def Redo():
		_buffer.Redo()
		
	def Undo():
		_buffer.Undo()

class MainWindow(Window):

	_status = Statusbar(HasResizeGrip: false)
	_notebookEditors = Notebook(TabPos: PositionType.Top, Scrollable: true)
	_notebookHelpers = Notebook(TabPos: PositionType.Bottom, Scrollable: true)
	_notebookOutline = Notebook(TabPos: PositionType.Bottom, Scrollable: true)
	
	_documentOutline = TreeView()
	
	_output = TextView(Editable: false)
	_outputBuffer = _output.Buffer
		
	_accelGroup = AccelGroup()	
	_editors = [] # workaround for gtk# bug #61703
	
	def constructor():
		super("Boo Explorer")
		
		self.AddAccelGroup(_accelGroup)
		self.Maximize()
		self.DeleteEvent += OnDelete		
		
		_documentOutline.AppendColumn("Name", CellRendererText (), ("text", 0))		
		_notebookOutline.AppendPage(CreateScrolled(_documentOutline), Label("Document Outline"))		
		_notebookHelpers.AppendPage(CreateScrolled(_output), Label("Output"))
				
		vbox = VBox(false, 2)
		vbox.PackStart(CreateMenuBar(), false, false, 0)		
		
		editPanel = VPaned()
		editPanel.Pack1(_notebookEditors, true, true)
		editPanel.Pack2(_notebookHelpers, false, true)	
		
		mainPanel = HPaned()
		mainPanel.Pack2(_notebookOutline, false, false)
		mainPanel.Pack1(editPanel, true, true)		
		
		vbox.PackStart(mainPanel, true, true, 0)
		vbox.PackStart(_status, false, false, 0)
		
		self.Add(vbox)
		
		self.NewDocument()
		
		Timeout.Add(3s.TotalMilliseconds, self.UpdateDocumentOutline)
		
	private def CreateScrolled(widget):
		sw = ScrolledWindow()
		sw.Add(widget)
		return sw
		
	private def AppendEditor(editor as BooEditor):
		_notebookEditors.AppendPage(editor, Label(editor.Label))
		_editors.Add(editor)
		editor.ShowAll()
		_notebookEditors.CurrentPage = _notebookEditors.NPages-1
		
	def NewDocument():
		self.AppendEditor(editor=BooEditor())
		return editor
		
	def OpenDocument(fname as string):
		editor = BooEditor()
		editor.Open(fname)
		self.AppendEditor(editor)
		return editor
		
	private def CreateMenuBar():
		mb = MenuBar()

		file = Menu()
		file.Append(ImageMenuItem(Stock.New, _accelGroup, Activated: _menuItemNew_Activated))
		file.Append(ImageMenuItem(Stock.Open, _accelGroup, Activated: _menuItemOpen_Activated))
		file.Append(ImageMenuItem(Stock.Save, _accelGroup, Activated: _menuItemSave_Activated))
		file.Append(SeparatorMenuItem())
		file.Append(ImageMenuItem(Stock.Quit, _accelGroup, Activated: _menuItemExit_Activated))
		
		edit = Menu()
		edit.Append(ImageMenuItem(Stock.Undo, _accelGroup, Activated: _menuItemUndo_Activated))
		edit.Append(ImageMenuItem(Stock.Redo, _accelGroup, Activated: _menuItemRedo_Activated))
		edit.Append(SeparatorMenuItem())
		edit.Append(ImageMenuItem(Stock.Cut, _accelGroup, Activated: _menuItemCut_Activated))
		edit.Append(ImageMenuItem(Stock.Copy, _accelGroup, Activated: _menuItemCopy_Activated))
		edit.Append(ImageMenuItem(Stock.Paste, _accelGroup, Activated: _menuItemPaste_Activated))
		edit.Append(ImageMenuItem(Stock.Delete, _accelGroup, Activated: _menuItemDelete_Activated))
		edit.Append(SeparatorMenuItem())
		edit.Append(ImageMenuItem(Stock.Preferences, _accelGroup))		
		
		tools = Menu()
		tools.Append(mi=ImageMenuItem(Stock.Execute, _accelGroup, Activated: _menuItemExecute_Activated))
		mi.AddAccelerator("activate", _accelGroup, AccelKey(Gdk.Key.F5, Enum.ToObject(Gdk.ModifierType, 0), AccelFlags.Visible))
		tools.Append(miExpand=MenuItem("Expand", Activated: _menuItemExpand_Activated))
		miExpand.AddAccelerator("activate", _accelGroup, AccelKey(Gdk.Key.E, Gdk.ModifierType.ControlMask, AccelFlags.Visible))
		
		documents = Menu()
		documents.Append(ImageMenuItem(Stock.Close, _accelGroup, Activated: _menuItemClose_Activated))
				
		mb.Append(MenuItem("_File", Submenu: file))
		mb.Append(MenuItem("_Edit", Submenu: edit))
		mb.Append(MenuItem("_Tools", Submenu: tools))
		mb.Append(MenuItem("_Documents", Submenu: documents))
		return mb
		
	CurrentEditor as BooEditor:
		get:
			// can't do the simpler:
			// editor as BooEditor = _notebookEditors.CurrentPageWidget
			// because of gtk# bug #61703
			return _editors[_notebookEditors.CurrentPage]
			
	def AppendOutput(text as string):
		target = _outputBuffer.EndIter
		_outputBuffer.Insert(target, text)
		
	def DisplayErrors(errors as CompilerErrorCollection):
		self.AppendOutput(errors.ToString(true)) if (len(errors))
		
	private def GetClipboard():
		return Clipboard.Get(Gdk.Selection.Clipboard)
		
	CurrentBuffer:
		get:
			return CurrentEditor.Buffer
			
	private def _menuItemClose_Activated():
		_editors.RemoveAt(_notebookEditors.CurrentPage)
		_notebookEditors.RemovePage(_notebookEditors.CurrentPage)
		
	private def _menuItemCut_Activated():	
		CurrentBuffer.CutClipboard(GetClipboard(), true)
		
	private def _menuItemCopy_Activated():	
		CurrentBuffer.CopyClipboard(GetClipboard())

	private def _menuItemPaste_Activated():	
		CurrentBuffer.PasteClipboard(GetClipboard())

	private def _menuItemDelete_Activated():	
		pass

	private def _menuItemExecute_Activated():	
		
		_outputBuffer.Clear()
		self.AppendOutput("${_outputBuffer.Text}****** Compiling ${CurrentEditor.Label} *******\n")	
		compiler = CreateCompiler(Boo.Lang.Compiler.Pipelines.Run())
		compiler.Parameters.Input.Add(StringInput(CurrentEditor.Label, CurrentEditor.Buffer.Text))
		
		start = date.Now
		try:
			using console=Boo.Lang.Interpreter.ConsoleCapture():
				result = compiler.Run()		
			self.DisplayErrors(result.Errors)
			self.AppendOutput(console.ToString())
		except x:
			self.AppendOutput(x.ToString())
		self.AppendOutput("Complete in ${date.Now-start}.")
		
	private def _menuItemNew_Activated():
		self.NewDocument()
				
	private def _menuItemOpen_Activated():
		fs = FileSelection("Open file", SelectMultiple: true)
		fs.Complete("*.boo")
		try:			
			if cast(int, ResponseType.Ok) == fs.Run():
				for fname in fs.Selections:
					self.OpenDocument(fname)
		ensure:
			fs.Hide()
			
		self.UpdateDocumentOutline()
		
	private def UpdateDocumentOutline():
		try:
			DocumentOutlineProcessor(_documentOutline, CurrentEditor).Update()			
		except ignored:
			pass
		return true // to match Gdk.Function signature
		
	private def _menuItemExpand_Activated():
		editor = CurrentEditor
		
		compiler = CreateCompiler(Boo.Lang.Compiler.Pipelines.CompileToBoo())
		compiler.Parameters.OutputWriter = StringWriter()
		compiler.Parameters.Input.Add(StringInput(editor.Label, editor.Buffer.Text))		
		result = compiler.Run()	
		self.DisplayErrors(result.Errors)
		unless len(result.Errors):		
			NewDocument().Buffer.Text = compiler.Parameters.OutputWriter.ToString()
			
	private def CreateCompiler(pipeline):
		compiler = BooCompiler()
		compiler.Parameters.Pipeline = pipeline
		compiler.Parameters.References.Add(System.Reflection.Assembly.LoadWithPartialName("nunit.framework"))
		return compiler
	
	private def _menuItemSave_Activated():
		editor = CurrentEditor
		
		fname = editor.FileName
		if fname is null:
			fs = FileSelection("Save As", SelectMultiple: false)
			if cast(int, ResponseType.Ok) != fs.Run():
				return
			fs.Hide()			
			fname = fs.Selections[0]			
		editor.SaveAs(fname)		
		_notebookEditors.SetTabLabelText(editor, editor.Label)
		
	private def _menuItemUndo_Activated():
		CurrentEditor.Undo()
	
	private def _menuItemRedo_Activated():
		CurrentEditor.Redo()
		
	private def _menuItemExit_Activated():
		Application.Quit()
		
	def OnDelete(sender, args as DeleteEventArgs):
		Application.Quit()
		args.RetVal = true
		
class DocumentOutlineProcessor:

	_store = TreeStore((string,))
	_documentOutline as TreeView
	_module as Boo.Lang.Compiler.Ast.Module
	
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
			iter = _store.AppendValues((type.Name,))
			if type isa TypeDefinition:
				UpdateType(iter, type)
		_documentOutline.Model = _store
		_documentOutline.ExpandAll()
				
	def UpdateType(parent, type as TypeDefinition):
		for member in type.Members:
			_store.AppendValues(parent, (member.Name,))			
	
		
Application.Init()

MainWindow().ShowAll()

Application.Run()
