namespace BooLog.Web

import BooLog
import System
import System.Web
import System.Web.UI
import System.Web.UI.WebControls

class DefaultPage(AbstractPage):
	
	_entries as Repeater
	
	override def OnInit(args as EventArgs):
		super(args)
		
		_entries.DataSource = BlogSystem.GetLatestEntries(10)
		_entries.DataBind()
		
	def GetEntryTitle(entry as BlogEntry):
		return entry.Title
		
	def GetEntryBody(entry as BlogEntry):
		return entry.Body
