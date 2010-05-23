"""
hooray!
"""
import Boo.Lang.Compiler
import Boo.Lang.Environments 

class ArbitraryService:
	override def ToString():
		return "hooray!"

With CompilerContext():
	assert my(ArbitraryService) is my(ArbitraryService)
	print my(ArbitraryService)
