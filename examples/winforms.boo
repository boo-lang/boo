import System
import System.Windows.Forms from System.Windows.Forms

class App:
	[getter(Times)]
	_times as int
	
	private def b_Click(sender, args as EventArgs):
		print("clicked!")	
		++_times
		
	private def f_Closed(sender, args as EventArgs):
		Application.Exit()
	
	def Run():
		b = Button(Text: "click me!", Click: b_Click)
		
		f = Form(Text: "My first boo winforms app", Closed: f_Closed)
		
		f.Controls.Add(b)
		f.Show()
		
		Application.Run(f)

app = App()
app.Run()
print("The button was clicked ${app.Times} times.")

