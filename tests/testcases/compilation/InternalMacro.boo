"""
Hello
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

class SayHelloMacro(AbstractAstMacro):
	override def Expand(node as MacroStatement):
		return [| print "Hello" |]
		
sayHello
