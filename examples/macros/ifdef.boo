"""
Will show the text below only if compiled with booc -d:BOO

woohoo BOO is defined
okthxbye
"""
import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

macro ifdef:
	if not ifdef.Arguments[0] isa StringLiteralExpression:
		raise ArgumentException("ifdef argument must be a string literal.")
	if Context.Parameters.Defines.ContainsKey((ifdef.Arguments[0] as StringLiteralExpression).Value):
		return [|
			$(ifdef.Block)
		|]
	
ifdef "BOO":
	print "woohoo BOO is defined"

print "okthxbye"
