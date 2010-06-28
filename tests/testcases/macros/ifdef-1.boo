"""
DEFINES: FOO
FOO
FOO or BAR
not BAR
DEFINES: BAR
BAR
FOO or BAR
not FOO
DEFINES: FOO, BAR
FOO
BAR
FOO and BAR
FOO or BAR
DEFINES: BAZ
not FOO
not BAR
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

import Boo.Lang.PatternMatching

def compileWithDefines(module as Module, *defines as (string)):
	compiler = BooCompiler()
	compiler.Parameters.Pipeline = Pipelines.CompileToMemory()
	compiler.Parameters.References.Add(System.Reflection.Assembly.GetExecutingAssembly())
	for define in defines:
		compiler.Parameters.Defines.Add(define, null)
	result = compiler.Run(CompileUnit(module.CloneNode()))
	assert len(result.Errors) == 0, result.Errors.ToString(true)
	return result.GeneratedAssembly
	
def runWithDefines(code as Module, *defines as (string)):
	print "DEFINES:", join(defines, ', ')
	return compileWithDefines(code, *defines).EntryPoint.Invoke(null, (null,))
	
macro printIfdef(expression as Expression):
	yield [|
		ifdef $expression:
			print $(expression.ToCodeString())
	|]
	
module = [|
	import Boo.Lang.ConditionalCompilation from Boo.Lang.Compiler

	printIfdef FOO
	printIfdef BAR
	printIfdef FOO and BAR
	printIfdef FOO or BAR
	printIfdef not FOO
	printIfdef not BAR
	
|]


runWithDefines module, "FOO"
runWithDefines module, "BAR"
runWithDefines module, "FOO", "BAR"
runWithDefines module, "BAZ"
	
