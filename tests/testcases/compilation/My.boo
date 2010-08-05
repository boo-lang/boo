"""
hooray!
"""
import Boo.Lang.Environments 

class ArbitraryService:
	override def ToString():
		return "hooray!"

Environment.With(ClosedEnvironment(ArbitraryService())):
	assert my(ArbitraryService) is my(ArbitraryService)
	print my(ArbitraryService)
