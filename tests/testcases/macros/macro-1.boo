"""
this should be executed twice
this should be executed twice
"""
### test for auto-import ### import Boo.Lang.Compiler

macro twice:
	return [|
		$(twice.Body)
		$(twice.Body)
	|]
	
twice:
	print "this should be executed twice"
