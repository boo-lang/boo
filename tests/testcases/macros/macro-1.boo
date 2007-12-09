"""
this should be executed twice
this should be executed twice
"""
import Boo.Lang.Compiler

macro twice:
	return [|
		$(twice.Block)
		$(twice.Block)
	|]
	
twice:
	print "this should be executed twice"
