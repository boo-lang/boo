
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps

def compile(code as string, references, count as int):
	compiler = BooCompiler()
	compiler.Parameters.OutputType = CompilerOutputType.Library
	compiler.Parameters.Input.Add(StringInput("<code${count}>", code))
	compiler.Parameters.Pipeline = CompileToMemory()
	for reference in references:
		compiler.Parameters.References.Add(reference)
	
	result = compiler.Run()
	assert len(result.Errors) == 0, result.Errors.ToString(true)
	return result.GetGeneratedAssembly()
	
library1 = """
class Foo:
	override def ToString():
		return 'Foo!'
"""

library2 = """
class Bar(Foo):
	override def ToString():
		return "\${super()} Bar!"
"""

asm1 = compile(library1, [], 1)
assert asm1 is not null

asm2 = compile(library2, [asm1], 2)
assert asm2 is not null

bar = asm2.GetType("Bar")
assert bar is not null
assert "Foo! Bar!" == bar().ToString()





