namespace BooExplorer

import WeifenLuo.WinFormsUI
import System
import System.Windows.Forms
import System.Drawing
import System.Text.RegularExpressions

class TaskList(Content):
	
	_list as ListView
	
	def constructor():
		_list = ListView(Dock: DockStyle.Fill,
						View: View.Details,
						FullRowSelect: true,
						GridLines: true)
					 
		_list.Columns.Add("line"       , 50, HorizontalAlignment.Left)
		_list.Columns.Add("column"     , 50, HorizontalAlignment.Left)
		_list.Columns.Add("code"       , 75, HorizontalAlignment.Left)
		_list.Columns.Add("description", 500, HorizontalAlignment.Left)
		_list.Columns.Add("module"     , 150, HorizontalAlignment.Left)

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
		_list.Items.Clear()

	def Add(itemText as string):
		parsedLine = ParseLine(itemText)
		item = _list.Items.Add(parsedLine[2])
		item.SubItems.AddRange((parsedLine[3], parsedLine[4], parsedLine[5], parsedLine[1]))
	
	def ParseLine(itemText as string):
		re = Regex("""(\S+)\((\d+),(\d+)\): (\w+): (.+)""")
		return array(string, [g.ToString() for g as Group in re.Match(itemText).Groups])
		
