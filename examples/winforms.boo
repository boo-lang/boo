import System.Windows.Forms from System.Windows.Forms

class App:
	public times as int
	
	def Run():
		b = Button(Text: "click me!")
		b.Click += def (sender, args):
			print("clicked!")	
			++times
		
		f = Form(Text: "My first boo winforms app")
		f.Closed += def (sender, args):
			Application.Exit()
		
		f.Controls.Add(b)
		f.Show()
		
		Application.Run(f)

app = App()
app.Run()
print("The button was clicked ${app.times} times.")

