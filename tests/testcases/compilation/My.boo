"""
hooray!
"""
import Boo.Lang.Compiler

class ArbitraryService:
	override def ToString():
		return "hooray!"

CompilerContext().Run:
	assert my(ArbitraryService) is my(ArbitraryService)
	print my(ArbitraryService)
