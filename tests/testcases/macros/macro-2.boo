""" 
[| 3 + 2 |]
"""
import Boo.Lang.Compiler

macro printcode:
	first, = printcode.Block.Statements
	return [|
		block:
			print $(first.ToCodeString())
	|].Block
	
printcode:
	[| 3 + 2 |]
