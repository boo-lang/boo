import System
import Gtk from "gtk-sharp"
import GtkSourceView from "gtksourceview-sharp"

class BooEditor(ScrolledWindow):

	static _booSourceLanguage = SourceLanguagesManager().GetLanguageFromMimeType("text/x-boo")
	
	_buffer = SourceBuffer(_booSourceLanguage, Highlight: true)	
	_view = SourceView(_buffer, ShowLineNumbers: true, AutoIndent: true)
	
	def constructor():
		super(Adjustment(IntPtr.Zero), Adjustment(IntPtr.Zero))				
		self.SetPolicy(PolicyType.Automatic, PolicyType.Automatic)
		self.Add(_view)
		
	Text:
		get:
			return _buffer.Text

class MainWindow(Window):

	_status = Statusbar(HasResizeGrip: false)
	_notebook = Notebook(TabPos: PositionType.Top, Scrollable: true)
	_accelGroup = AccelGroup()

	def constructor():
		super("Boo Explorer")
		
		self.AddAccelGroup(_accelGroup)
		self.SetDefaultSize(600, 400)
		self.DeleteEvent += OnDelete		
				
		vbox = VBox(false, 1)
		vbox.PackStart(CreateMenuBar(), false, false, 0)
		vbox.PackStart(_notebook, true, true, 0)
		vbox.PackStart(_status, false, false, 0)
		
		self.Add(vbox)
		
		self.NewDocument()
		
	def NewDocument():
		editor = BooEditor()
		_notebook.AppendPage(editor, Label("unnamed.boo"))
		editor.ShowAll()
		
	private def CreateMenuBar():
		mb = MenuBar()

		file = Menu()
		file.Append(ImageMenuItem(Stock.New, _accelGroup, Activated: _menuItemNew_Activated))
		file.Append(ImageMenuItem(Stock.Open, _accelGroup, Activated: _menuItemOpen_Activated))
		file.Append(ImageMenuItem(Stock.Save, _accelGroup, Activated: _menuItemSave_Activated))
		file.Append(SeparatorMenuItem())
		file.Append(ImageMenuItem(Stock.Quit, _accelGroup, Activated: _menuItemExit_Activated))
		
		edit = Menu()
		edit.Append(ImageMenuItem(Stock.Undo, _accelGroup))
		edit.Append(ImageMenuItem(Stock.Redo, _accelGroup))
		edit.Append(SeparatorMenuItem())
		edit.Append(ImageMenuItem(Stock.Cut, _accelGroup))
		edit.Append(ImageMenuItem(Stock.Copy, _accelGroup))
		edit.Append(ImageMenuItem(Stock.Paste, _accelGroup))
		edit.Append(ImageMenuItem(Stock.Delete, _accelGroup))
		edit.Append(SeparatorMenuItem())
		edit.Append(ImageMenuItem(Stock.Preferences, _accelGroup))
		
		tools = Menu()
		tools.Append(ImageMenuItem(Stock.Execute, _accelGroup, Activated: _menuItemExecute_Activated))
				
		mb.Append(MenuItem("File", Submenu: file))
		mb.Append(MenuItem("Edit", Submenu: edit))
		mb.Append(MenuItem("Tools", Submenu: tools))
		return mb
		
	private def _menuItemExecute_Activated(sender, args as EventArgs):
		// trying to track down a gtk-sharp bug
		// after a few executions (garbage collection?)
		// CurrentPageWidget will start to return a simple
		// ScrolledWindow (no longer a BooEditor)
		print(_notebook.CurrentPage)
		print(_notebook.CurrentPageWidget)
		editor = _notebook.CurrentPageWidget as BooEditor
		return if editor is null
		
		compiler = Boo.Lang.Compiler.BooCompiler()
		compiler.Parameters.Input.Add(Boo.Lang.Compiler.IO.StringInput("<script>", editor.Text))
		compiler.Parameters.Pipeline = Boo.Lang.Compiler.Pipelines.Run()
		
		result = compiler.Run()
		print(result.Errors.ToString()) if len(result.Errors)
		
	private def _menuItemNew_Activated(sender, args as EventArgs):
		self.NewDocument()
				
	private def _menuItemOpen_Activated(sender, args as EventArgs):
		pass
	
	private def _menuItemSave_Activated(sender, args as EventArgs):
		pass
		
	private def _menuItemExit_Activated(sender, args as EventArgs):
		Application.Quit()
		
	def OnDelete(sender, args as DeleteEventArgs):
		Application.Quit()
		args.RetVal = true
		
Application.Init()

MainWindow().ShowAll()

Application.Run()
