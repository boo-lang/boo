namespace BooExplorer

import WeifenLuo.WinFormsUI
import System
import System.Windows.Forms
import System.Drawing

class OutputPane(DockContent):
	
	_richBox as RichTextBox
	
	def constructor():
		_richBox = RichTextBox(Dock: DockStyle.Fill,
								Multiline: true,
								ReadOnly: true)
		SuspendLayout()
		
		Controls.Add(_richBox)
		self.HideOnClose = true
		self.DockableAreas = (
					DockAreas.Float |
					DockAreas.DockBottom |
					DockAreas.DockTop |
					DockAreas.DockLeft |
					DockAreas.DockRight)
		self.ClientSize = System.Drawing.Size(295, 347)
		self.ShowHint = DockState.DockBottom
		self.Text = "Output"

		ResumeLayout(false)

	def SetBuildText(text as string):
		_richBox.Text = text
		
	override protected def GetPersistString():
		return "OutputPane|"

