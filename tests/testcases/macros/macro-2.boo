""" 
[| 3 + 2 |]
"""
import Boo.Lang.Compiler

macro printcode:
	first, = printcode.Body.Statements
	return [|
		block:
			print $(first.ToCodeString())
	|].Body
	
printcode:
	[| 3 + 2 |]
