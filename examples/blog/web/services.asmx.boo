namespace BooLog.Web

import System
import System.Web
import System.Web.Services
import BooLog

class BooLogServices(WebService):

	BlogSystem:
		get:
			return cast(BooLogApplication, Context.ApplicationInstance).BlogSystem
		

