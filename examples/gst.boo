import System
import Gtk from "gtk-sharp"
import GtkSourceView from "gtksourceview-sharp"

def window_Delete(sender, args as DeleteEventArgs):
	Application.Quit()
	args.RetVal = true

Application.Init()
	
booSourceLanguage = SourceLanguagesManager().GetLanguageFromMimeType("text/x-boo")
buffer = SourceBuffer(booSourceLanguage, Highlight: true)	
sourceView = SourceView(buffer, ShowLineNumbers: true, AutoIndent: true)

window = Window("Simple Boo Editor",
				DefaultWidth:  600,
				DefaultHeight: 400,
				DeleteEvent: window_Delete)
window.Add(sourceView)
window.ShowAll()

Application.Run()
