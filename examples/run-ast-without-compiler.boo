import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

// print('Hello!')
mie = MethodInvocationExpression(ReferenceExpression("print"))
mie.Arguments.Add(StringLiteralExpression("Hello!"))

// statements and expressions must be inside a code block
module = Module()
module.Globals.Add(mie)

// modules must be inside a CompileUnit
cunit = CompileUnit()
cunit.Modules.Add(module)

pipeline = Pipelines.Run()
pipeline.Run(CompilerContext(cunit))
