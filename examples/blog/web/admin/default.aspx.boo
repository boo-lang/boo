namespace BooLog.Web.Admin

import System
import System.Web
import System.Web.UI
import System.Web.UI.WebControls
import BooLog

class DefaultPage(BooLog.Web.AbstractPage):
	
	_entries as DataGrid
	
	override def OnInit(args as EventArgs):
		super(args)
		
		_entries.DataSource = BlogSystem.Entries
		_entries.DataBind()
		
	def GetTitle(entry as BlogEntry):
		return entry.Title
		
	def GetDatePosted(entry as BlogEntry):
		return entry.DatePosted
