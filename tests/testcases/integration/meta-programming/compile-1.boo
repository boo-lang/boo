"""
it lives!
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Pipelines

module = Module()
module.Globals.Add(
	MethodInvocationExpression(
		ReferenceExpression("print"),
		StringLiteralExpression("it lives!")))

cu = CompileUnit()
cu.Modules.Add(module)

compiler = BooCompiler()
compiler.Parameters.Pipeline = Run()

result = compiler.Run(cu)
assert 0 == len(result.Errors)
