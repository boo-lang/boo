import System
import Gtk

def button_Clicked(sender, args as EventArgs):
	print("button clicked!")
	
def window_Delete(sender, args as DeleteEventArgs):
	Application.Quit()
	args.RetVal = true

Application.Init()

window = Window("Button Tester",
				DefaultWidth:  200,
				DefaultHeight: 150,
				DeleteEvent: window_Delete)
				
window.Add(Button("Click Me!", Clicked: button_Clicked))
window.ShowAll()

Application.Run()
