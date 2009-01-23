"""
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

macro setNoWarn:
	Context.Parameters.NoWarn = true

setNoWarn
x = 1
x = x #BCW0020
print 1 == 1 #BCW0022

