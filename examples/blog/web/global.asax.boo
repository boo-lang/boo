namespace BooLog.Web

import System
import System.IO
import System.Web
import Bamboo.Prevalence

class BooLogApplication(HttpApplication):
	
	static _engine as PrevalenceEngine
	
	BlogSystem as BooLog.BlogSystem:
		get:
			return _engine.PrevalentSystem
	
	def Application_Start():
		baseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data")
		_engine = PrevalenceActivator.CreateTransparentEngine(BooLog.BlogSystem, baseFolder)		
		
	def Application_End():
		_engine.TakeSnapshot()
