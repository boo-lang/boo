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
			print $(macro.Body.ToCodeString())
			$(macro.Body)
		|]

class TwiceMacro(MacroBase):
	override def Expand(macro as MacroStatement):
		return [|
			$(super.Expand(macro))
			$(super.Expand(macro))
		|]

macro test:
	return [|
		$(test.Body)
	|]

def foo():
	a = 0
	twice:
		test:
			a += 4
	print a

foo()
