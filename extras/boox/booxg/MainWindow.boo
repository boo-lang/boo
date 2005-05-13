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
		
		DocumentOutlineProcessor.SetUp(_documentOutline)
		_notebookOutline.AppendPage(CreateScrolled(_documentOutline), Label("Document Outline"))
		_notebookOutline.AppendPage(CreateScrolled(CreateFileChooser()), Label("File System"))		
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
		pageIndex = _notebookEditors.AppendPage(editor, Label(editor.Label))
		page = _notebookEditors.GetNthPage(pageIndex)
		editor.LabelChanged += def():
			_notebookEditors.SetTabLabelText(page, editor.Label)
		_editors.Add(editor)
		editor.ShowAll()
		_notebookEditors.CurrentPage = _notebookEditors.NPages-1
		
	def NewDocument():
		self.AppendEditor(editor=BooEditor())
		return editor
		
	def OpenDocument(fname as string):
		fname = System.IO.Path.GetFullPath(fname)
		
		i = 0
		for editor as BooEditor in _editors:
			if fname == editor.FileName:
				_notebookEditors.CurrentPage = i
				return
			++i
				
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
			return _editors[_notebookEditors.CurrentPage] if len(_editors)
			
	def AppendOutput(text as string):
		target = _outputBuffer.EndIter
		_outputBuffer.Insert(target, text)
		
	def DisplayErrors(errors as CompilerErrorCollection):
		self.AppendOutput(errors.ToString(true)) if (len(errors))
		
	private def GetClipboard():
		return Clipboard.Get(Gdk.Selection.Clipboard)
		
	CurrentBuffer:
		get:
			return null if CurrentEditor is null
			return CurrentEditor.Buffer
			
	private def _menuItemClose_Activated():
		page = _notebookEditors.CurrentPage
		_notebookEditors.RemovePage(page)
		_editors.RemoveAt(page)
		
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
		fs = FileChooserDialog("Open file", self, FileChooserAction.Open, (,))
		SetUpFileChooser(fs)
		fs.Run()
		fs.Hide()
		
	private def CreateFileChooser():
		fs = FileChooserWidget(FileChooserAction.Open)
		SetUpFileChooser(fs)
		return fs
		
	private def SetUpFileChooser(fs as FileChooser):
		filter = FileFilter(Name: "Boo Files (*.boo)")
		filter.AddPattern("*.boo")
		fs.AddFilter(filter) 
		filter = FileFilter(Name: "All Files (*.*)")
		filter.AddPattern("*.*")
		fs.AddFilter(filter)
		fs.FileActivated += def():
			self.OpenDocument(fs.Filename)
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
