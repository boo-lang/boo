import System
import Gtk from "gtk-sharp"
import GtkSourceView from "gtksourceview-sharp"

class MainWindow(Window):

	_status = Statusbar(HasResizeGrip: false)
	
	_booSourceLanguage = SourceLanguagesManager().GetLanguageFromMimeType("text/x-boo")

	def constructor():
		super("Boo Explorer")
		
		self.SetDefaultSize(600, 400)
		self.DeleteEvent += OnDelete
		
		buffer = SourceBuffer(_booSourceLanguage, Highlight: true)	
		sourceView = SourceView(buffer, ShowLineNumbers: true, AutoIndent: true)
		
		scrolledWindow = ScrolledWindow(Adjustment(IntPtr.Zero), Adjustment(IntPtr.Zero))
		scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic)
		scrolledWindow.Add(sourceView)
				
		vbox = VBox(false, 1)
		vbox.Add(CreateMenuBar())
		vbox.Add(scrolledWindow)
		vbox.Add(_status)
		
		self.Add(vbox)
		
	private def CreateMenuBar():
		mb = MenuBar()

		file = Menu()
		file.Append(MenuItem("Exit", Activated: _menuItemExit_Activated))
				
		mb.Append(MenuItem("File", Submenu: file))
		return mb
		
	private def _menuItemExit_Activated(sender, args as EventArgs):
		Application.Quit()
		
	def OnDelete(sender, args as DeleteEventArgs):
		Application.Quit()
		args.RetVal = true
		
Application.Init()

MainWindow().ShowAll()

Application.Run()
