import System
import Gtk from "gtk-sharp"
import GtkSourceView from "gtksourceview-sharp"

def window_Delete(sender, args as DeleteEventArgs):
	Application.Quit()
	args.RetVal = true

Application.Init()

window = Window("Simple Boo Editor",
				DefaultWidth:  600,
				DefaultHeight: 400,
				DeleteEvent: window_Delete)
	
boo = SourceLanguagesManager().GetLanguageFromMimeType("text/x-boo")
buffer = SourceBuffer(boo, Highlight: true)	
sourceView = SourceView(buffer)
				
window.Add(sourceView)
window.ShowAll()

Application.Run()
