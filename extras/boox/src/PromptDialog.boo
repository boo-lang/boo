namespace BooExplorer

import System.Windows.Forms
import System.Drawing
import System

class PromptDialog(Form):
	
	_value as TextBox
	_message as Label
	
	def constructor():		
		
		_message = Label(Location: Point(2, 2),
						Size: System.Drawing.Size(200, 18))
		_value = TextBox(
						Location: Point(2, 20),
						Size: System.Drawing.Size(290, 18))
						
		ok = Button(Text: "OK",
					Location: Point(50, 45),
					DialogResult: DialogResult.OK)
					
		cancel = Button(Text: "Cancel",
					Location: Point(150, 45),
					DialogResult: DialogResult.Cancel)
		
		SuspendLayout()
		self.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		self.StartPosition = FormStartPosition.CenterParent
		self.Size = System.Drawing.Size(300, 100)
		self.AcceptButton = ok
		self.CancelButton = cancel
		Controls.Add(_message)
		Controls.Add(_value)		
		Controls.Add(ok)
		Controls.Add(cancel)
		ResumeLayout(false)
		
	Message as string:
		set:
			_message.Text = value
			
	Value:
		get:
			return _value.Text
		set:
			_value.Text = value

def test():	
	dlg = PromptDialog(Text: "Boo Explorer", Message: "Hello? ")
	result = dlg.ShowDialog()
	print(result)
	if DialogResult.OK == result:
		print(dlg.Value)

//test()
