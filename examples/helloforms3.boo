import System.Windows.Forms from System.Windows.Forms

f = Form(Text: "Hello!")
f.Controls.Add(Button(Text: "Click Me!", Dock: DockStyle.Fill))

Application.Run(f)
