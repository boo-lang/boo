using System.Windows.Forms from System.Windows.Forms

def b_click(sender, args as EventArgs):
	print("clicked!")

f = Form(Text: "Hello, Boo!")
b = Button(Text: "Click Me!")
b.Click = b_click

f.Controls.Add(b)

Application.Run(f)
