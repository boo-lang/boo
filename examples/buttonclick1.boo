import System.Windows.Forms from System.Windows.Forms

def clicked(sender, args as System.EventArgs):
	print("clicked!")

f = Form(Text: "Hello, Boo!")
f.Controls.Add(Button(Text: "Click Me!",
					Dock: DockStyle.Fill,
					Click: clicked ))

Application.Run(f)
