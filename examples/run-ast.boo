import Boo.Lang.Compiler
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Ast

class CodeGeneratorStep(AbstractCompilerStep):
	
	override def Run():
		
		// print('Hello!')
		mie = MethodInvocationExpression(ReferenceExpression("print"))
		mie.Arguments.Add(StringLiteralExpression("Hello!"))
		
		module = Module()
		module.Globals.Add(mie)
		
		CompileUnit.Modules.Add(module)

compiler = BooCompiler()
compiler.Parameters.Pipeline = Run()
compiler.Parameters.Pipeline.Insert(0, CodeGeneratorStep())

result = compiler.Run()
for error in result.Errors:
	print(error)
