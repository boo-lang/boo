namespace BooLog.Web

abstract class AbstractPage(System.Web.UI.Page):

	BlogSystem:
		get:
			return cast(BooLogApplication, Context.ApplicationInstance).BlogSystem
