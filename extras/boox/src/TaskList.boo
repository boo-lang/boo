namespace BooExplorer

import WeifenLuo.WinFormsUI
import System
import System.Windows.Forms
import System.Drawing

class TaskList(Content):
	
	_list as ListView
	
	def constructor():
		_list = ListView(Dock: DockStyle.Fill,
						View: View.List)
						
		SuspendLayout()
		
		Controls.Add(_list)
		self.AllowedStates = (
					ContentStates.Float |
					ContentStates.DockBottom |
					ContentStates.DockTop |
					ContentStates.DockLeft |
					ContentStates.DockRight)
		self.ClientSize = System.Drawing.Size(295, 347)
		self.HideOnClose = true
		self.ShowHint = DockState.DockBottom
		self.Text = "Task List"
		ResumeLayout(false)
		
	def Clear():
		_list.Clear()
		
	def Add(itemText as string):
		_list.Items.Add(itemText)
