namespace BooExplorer

import WeifenLuo.WinFormsUI
import System
import System.Windows.Forms
import System.Drawing

class OutputPane(Content):
	
	_richBox as RichTextBox
	
	def constructor():
		_richBox = RichTextBox(Dock: DockStyle.Fill,
								Multiline: true,
								ReadOnly: true)
		SuspendLayout()
		
		Controls.Add(_richBox)
		self.AllowedStates = (
					ContentStates.Float |
					ContentStates.DockBottom |
					ContentStates.DockTop |
					ContentStates.DockLeft |
					ContentStates.DockRight)
		self.ClientSize = System.Drawing.Size(295, 347)
		self.HideOnClose = true
		self.ShowHint = DockState.DockBottom
		self.Text = "Output"

		ResumeLayout(false)

	def SetBuildText(text as string):
		_richBox.Text = text

