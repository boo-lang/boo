import System
import Boo.IO.TextFile
import Gdk from "gdk-sharp" as Gdk
import Gtk from "gtk-sharp"
import GtkSourceView from "gtksourceview-sharp"

class BooEditor(ScrolledWindow):

	static _booSourceLanguage = SourceLanguagesManager().GetLanguageFromMimeType("text/x-boo")
	
	_buffer = SourceBuffer(_booSourceLanguage, Highlight: true)	
	_view = SourceView(_buffer, ShowLineNumbers: true, AutoIndent: true)
	
	[getter(FileName)]
	_fname as string
	
	def constructor([required] fname as string):
		_fname = fname
		self.SetPolicy(PolicyType.Automatic, PolicyType.Automatic)
		self.Add(_view)
		
	Text:
		get:
			return _buffer.Text
			
		set:
			_buffer.Text = value
			
	def Redo():
		_buffer.Redo()
		
	def Undo():
		_buffer.Undo()

class MainWindow(Window):

	_status = Statusbar(HasResizeGrip: false)
	_notebookEditors = Notebook(TabPos: PositionType.Top, Scrollable: true)
	_notebookHelpers = Notebook(TabPos: PositionType.Top, Scrollable: true)
	_notebookOutline = Notebook(TabPos: PositionType.Bottom, Scrollable: true)
	_documentOutline = TreeView()
	_accelGroup = AccelGroup()	
	_editors = [] # workaround for gtk# bug #61703
	
	def constructor():
		super("Boo Explorer")
		
		self.AddAccelGroup(_accelGroup)
		self.SetDefaultSize(600, 400)
		self.DeleteEvent += OnDelete		
		
		_notebookOutline.AppendPage(_documentOutline, Label("Document Outline"))
				
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
		
	private def AppendEditor(editor as BooEditor):
		_notebookEditors.AppendPage(editor, Label(editor.FileName))
		_editors.Add(editor)
		editor.ShowAll()
		
	def NewDocument():
		self.AppendEditor(BooEditor("unnamed.boo"))
		
	def OpenDocument(fname as string):
		self.AppendEditor(BooEditor(fname, Text: ReadFile(fname)))
		
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
		mi.AddAccelerator("activate", _accelGroup, AccelKey(Gdk.Key.F5, Gdk.ModifierType.ControlMask, AccelFlags.Visible))
		
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
		
	private def _menuItemExecute_Activated(sender, args as EventArgs):	
		
		compiler = Boo.Lang.Compiler.BooCompiler()
		compiler.Parameters.Input.Add(
				Boo.Lang.Compiler.IO.StringInput("<script>", CurrentEditor.Text))
		compiler.Parameters.Pipeline = Boo.Lang.Compiler.Pipelines.Run()
		
		result = compiler.Run()
		print(result.Errors.ToString()) if len(result.Errors)
		
	private def _menuItemNew_Activated(sender, args as EventArgs):
		self.NewDocument()
				
	private def _menuItemOpen_Activated(sender, args as EventArgs):
		fs = FileSelection("Select a file", SelectMultiple: true)
		try:
			if cast(int, ResponseType.Ok) == fs.Run():
				for fname in fs.Selections:
					self.OpenDocument(fname)
		ensure:
			fs.Hide()
	
	private def _menuItemSave_Activated(sender, args as EventArgs):
		pass
		
	private def _menuItemUndo_Activated(sender, args as EventArgs):
		CurrentEditor.Undo()
	
	private def _menuItemRedo_Activated(sender, args as EventArgs):
		CurrentEditor.Redo()
		
	private def _menuItemExit_Activated(sender, args as EventArgs):
		Application.Quit()
		
	def OnDelete(sender, args as DeleteEventArgs):
		Application.Quit()
		args.RetVal = true
		
Application.Init()

MainWindow().ShowAll()

Application.Run()
