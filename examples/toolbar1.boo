using System.Windows.Forms from System.Windows.Forms

f = Form(Text: "Hello, Boo!")

tb = ToolBar(
			ShowToolTips: true,
			TabIndex: 0,
			Appearance: ToolBarAppearance.Flat,
			Cursor: Cursors.Hand)
			
tb.Buttons.Add(ToolBarButton(Text: "Click Me!",
							ToolTipText: "You heard me."))

f.Controls.Add(tb)

Application.Run(f)
