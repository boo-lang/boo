namespace MetaBoo.PipelineSteps

import MetaBoo
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Ast.Visitors

class PrintBoo(AbstractCompilerPipelineStep):
	
	override def Run():
		BooPrinterVisitor(System.Console.Out).Switch(self.CompileUnit)
