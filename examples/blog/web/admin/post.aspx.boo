namespace BooLog.Web.Admin

import System
import System.Web
import System.Web.UI
import System.Web.UI.WebControls
import BooLog

class PostPage(BooLog.Web.AbstractPage):

	_title as TextBox
	_body as TextBox
	
	def _post_Click(sender, args as EventArgs):
		return unless Page.IsValid
		
		entry = BlogEntry(_title.Text, _body.Text)
		BlogSystem.Post(entry)
		
		Response.Redirect("default.aspx")
