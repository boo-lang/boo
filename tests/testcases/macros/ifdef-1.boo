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
DEFINES: FOO=foo, QUX=qux
FOO
FOO or BAR
not BAR
FOO == 'foo'
(QUX == qux) or (QUX == 10)
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

def compileWithDefines(module as Module, *defines as (string)):
	compiler = BooCompiler()
	compiler.Parameters.Pipeline = Pipelines.CompileToMemory()
	compiler.Parameters.References.Add(System.Reflection.Assembly.GetExecutingAssembly())
	for define in defines:
		pair = define.Split("=".ToCharArray(), 2)
		if len(pair) == 2:
			compiler.Parameters.Defines.Add(pair[0], pair[1])
		else:
			compiler.Parameters.Defines.Add(pair[0], null)

	result = compiler.Run(CompileUnit(module.CloneNode()))
	assert len(result.Errors) == 0, result.Errors.ToString(true)
	return result.GetGeneratedAssembly()
	
def runWithDefines(code as Module, *defines as (string)):
	print "DEFINES:", join(defines, ', ')
	return compileWithDefines(code, *defines).GetEntryPoint().Invoke(null, (null,))
	
macro printIfdef(expression as Expression):
	yield [|
		ifdef $expression:
			print $(expression.ToCodeString())
	|]
	
module = [|
	import System
	
	printIfdef FOO
	printIfdef BAR
	printIfdef FOO and BAR
	printIfdef FOO or BAR
	printIfdef not FOO
	printIfdef not BAR
	printIfdef FOO == 'foo'
	printIfdef QUX == qux or QUX == 10
|]


runWithDefines module, "FOO"
runWithDefines module, "BAR"
runWithDefines module, "FOO", "BAR"
runWithDefines module, "BAZ"
runWithDefines module, "FOO=foo", "QUX=qux"
