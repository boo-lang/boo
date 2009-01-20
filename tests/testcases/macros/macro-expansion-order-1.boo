"""
using
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

macro printParentMacroName:
	parentName = printParentMacroName.GetAncestor[of MacroStatement]().Name.ToString()
	yield [| print $parentName |]
	
using null:
	printParentMacroName
	

