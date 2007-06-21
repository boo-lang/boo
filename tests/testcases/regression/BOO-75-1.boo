import NUnit.Framework
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps

def compile(code as string, references):	
	compiler = BooCompiler()
	compiler.Parameters.OutputType = CompilerOutputType.Library
	compiler.Parameters.Input.Add(StringInput("<code>", code))
	compiler.Parameters.Pipeline = CompileToMemory()
	for reference in references:
		compiler.Parameters.References.Add(reference)
	
	result = compiler.Run()
	Assert.Fail(result.Errors.ToString(true)) if len(result.Errors)
	return result.GeneratedAssembly	
	
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

asm1 = compile(library1, [])
Assert.IsNotNull(asm1)

asm2 = compile(library2, [asm1])
Assert.IsNotNull(asm2)

bar = asm2.GetType("Bar")
Assert.IsNotNull(bar)
Assert.AreEqual("Foo! Bar!", bar().ToString())





