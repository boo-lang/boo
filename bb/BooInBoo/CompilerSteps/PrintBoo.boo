namespace BooInBoo.CompilerSteps

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Ast.Visitors
import Boo.Lang.Compiler.Pipeline

class PrintBoo(AbstractCompilerStep):
	
	override def Run():
		BooPrinterVisitor(System.Console.Out).Switch(CompileUnit)
