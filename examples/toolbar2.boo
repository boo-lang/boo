import System.Windows.Forms from System.Windows.Forms

def tb_Click(sender, args as System.EventArgs):
	MessageBox.Show("Cool or what?")

f = Form(Text: "Hello, Boo!")

tb = ToolBar(ShowToolTips: true,
			TabIndex: 0,
			Appearance: ToolBarAppearance.Flat,
			Cursor: Cursors.Hand,
			Click: tb_Click)
			
tb.Buttons.Add(ToolBarButton(Text: "Click Me!", ToolTipText: "You heard me."))

f.Controls.Add(tb)

Application.Run(f)
