import System
import System.IO
import Boo.IO
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Ast
import Gdk from "gdk-sharp" as Gdk
import Gtk from "gtk-sharp"
import GtkSourceView from "gtksourceview-sharp"
import BooExplorer.Common from BooExplorer.Common

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
	
	_output = TextView()
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
				
		vbox = VBox(false, 1)
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
		edit.Append(ImageMenuItem(Stock.Cut, _accelGroup))
		edit.Append(ImageMenuItem(Stock.Copy, _accelGroup))
		edit.Append(ImageMenuItem(Stock.Paste, _accelGroup))
		edit.Append(ImageMenuItem(Stock.Delete, _accelGroup))
		edit.Append(SeparatorMenuItem())
		edit.Append(ImageMenuItem(Stock.Preferences, _accelGroup))		
		
		tools = Menu()
		tools.Append(mi=ImageMenuItem(Stock.Execute, _accelGroup, Activated: _menuItemExecute_Activated))
		mi.AddAccelerator("activate", _accelGroup, AccelKey(Gdk.Key.F5, Enum.ToObject(Gdk.ModifierType, 0), AccelFlags.Visible))
		tools.Append(miExpand=MenuItem("Expand", Activated: _menuItemExpand_Activated))
		miExpand.AddAccelerator("activate", _accelGroup, AccelKey(Gdk.Key.E, Gdk.ModifierType.ControlMask, AccelFlags.Visible))
		
		documents = Menu()
		documents.Append(ImageMenuItem(Stock.Close, _accelGroup))
				
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
		_outputBuffer.Insert(_outputBuffer.EndIter, text)
		
	def DisplayErrors(errors as CompilerErrorCollection):
		self.AppendOutput(errors.ToString(true)) if (len(errors))
		
	private def _menuItemExecute_Activated(sender, args as EventArgs):	
		
		_outputBuffer.Clear()
		self.AppendOutput("${_outputBuffer.Text}****** Compiling ${CurrentEditor.Label} *******\n")	
		compiler = Boo.Lang.Compiler.BooCompiler()
		compiler.Parameters.Input.Add(StringInput(CurrentEditor.Label, 								CurrentEditor.Buffer.Text))
		compiler.Parameters.Pipeline = Boo.Lang.Compiler.Pipelines.Run()
		
		start = date.Now
		using console=ConsoleCapture():
			result = compiler.Run()		
		self.DisplayErrors(result.Errors)
		self.AppendOutput(console.ToString())
		self.AppendOutput("Complete in ${date.Now-start}.")
		
	private def _menuItemNew_Activated(sender, args as EventArgs):
		self.NewDocument()
				
	private def _menuItemOpen_Activated(sender, args as EventArgs):
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
		DocumentOutlineProcessor(_documentOutline, CurrentEditor).Update()	
		
	private def _menuItemExpand_Activated(sender, args as EventArgs):
		editor = CurrentEditor
		
		compiler = BooCompiler()
		compiler.Parameters.OutputWriter = StringWriter()
		compiler.Parameters.Pipeline = Boo.Lang.Compiler.Pipelines.CompileToBoo()
		compiler.Parameters.Input.Add(StringInput(editor.Label, editor.Buffer.Text))		
		result = compiler.Run()	
		self.DisplayErrors(result.Errors)
		unless len(result.Errors):		
			NewDocument().Buffer.Text = compiler.Parameters.OutputWriter.ToString()
	
	private def _menuItemSave_Activated(sender, args as EventArgs):
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
		
	private def _menuItemUndo_Activated(sender, args as EventArgs):
		CurrentEditor.Undo()
	
	private def _menuItemRedo_Activated(sender, args as EventArgs):
		CurrentEditor.Redo()
		
	private def _menuItemExit_Activated(sender, args as EventArgs):
		Application.Quit()
		
	def OnDelete(sender, args as DeleteEventArgs):
		Application.Quit()
		args.RetVal = true
		
class DocumentOutlineProcessor:

	_store = TreeStore((string,))
	_documentOutline as TreeView
	_module as Boo.Lang.Compiler.Ast.Module
	
	def constructor(documentOutline, editor as BooEditor):
		_module = Parse(editor.FileName, editor.Buffer.Text)
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
			iter = _store.AppendValues(parent, (member.Name,))			
	
		
Application.Init()

MainWindow().ShowAll()

Application.Run()
