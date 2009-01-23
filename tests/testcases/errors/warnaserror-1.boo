"""
warnaserror-1.boo(12,3): BCW0020: WARNING: Assignment made to same expression. Did you mean to assign to something else?
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

macro setWarnAsError:
	Context.Parameters.WarnAsError = true

setWarnAsError
x = 1
x = x #BCW0020 => error

