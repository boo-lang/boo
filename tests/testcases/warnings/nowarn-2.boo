"""
nowarn-2.boo(13,3): BCW0020: WARNING: Assignment made to same expression. Did you mean to assign to something else?
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

macro setNoWarn:
	Context.Parameters.DisableWarning("BCW0016")
	Context.Parameters.DisableWarning("BCW0022")

setNoWarn
x = 1
x = x #BCW0020
print 1 == 1 #BCW0022

