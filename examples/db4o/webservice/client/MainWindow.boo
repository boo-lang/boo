import System
import Gtk

class MainWindow(Window):
	def constructor(title as string):
		super(title)
		SetDefaultSize(400, 300)
		DeleteEvent += { Application.Quit() }

Application.Init()
w = MainWindow("Boo WebService Client")
w.ShowAll()
Application.Run()