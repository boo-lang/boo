"""
a += 4

a += 4

8
"""

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

abstract class MacroBase(AbstractAstMacro):
	override def Expand(macro as MacroStatement):
		return [|
			print $(macro.Block.ToCodeString())
			$(macro.Block)
		|]

class TwiceMacro(MacroBase):
	override def Expand(macro as MacroStatement):
		return [|
			$(super.Expand(macro))
			$(super.Expand(macro))
		|]

macro test:
	return [|
		$(test.Block)
	|]

def foo():
	a = 0
	twice:
		test:
			a += 4
	print a

foo()