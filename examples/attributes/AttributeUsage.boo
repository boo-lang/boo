import Boo.Web
import System.Web.UI

class MyControl(UserControl):

	[ViewState(Default: 70)]
	Value as int
	
	[ViewState]
	Text as string
