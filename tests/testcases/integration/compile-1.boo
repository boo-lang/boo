"""
it lives!
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Pipelines

code = Block()
code.Add(
	MethodInvocationExpression(
		ReferenceExpression("print"),
		(StringLiteralExpression("it lives!"),)))

cu = CompileUnit()
cu.Modules.Add(Module(code))

compiler = BooCompiler()
compiler.Parameters.Pipeline = Run()

result = compiler.Run(cu)
assert 0 == len(result.Errors)
