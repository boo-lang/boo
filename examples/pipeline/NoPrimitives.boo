import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem

class CustomTypeSystem(TypeSystemServices):
	def constructor(context as CompilerContext):
		super(context)
		
	override def PreparePrimitives():
		self.AddPrimitiveType("string", self.StringType)
		self.AddPrimitiveType("void", self.VoidType)
		
class InitializeCustomTypeSystem(AbstractCompilerStep):
	override def Run():
		self.Context.TypeSystemServices = CustomTypeSystem(self.Context)

pipeline = Pipelines.CompileToMemory()
pipeline.Replace(InitializeTypeSystemServices, InitializeCustomTypeSystem())
pipeline.RemoveAt(pipeline.Find(IntroduceGlobalNamespaces))

code = """
import System.Console
WriteLine(date.Now)
WriteLine(List())
"""

compiler = BooCompiler()
compiler.Parameters.Input.Add(IO.StringInput("code.boo", code))
compiler.Parameters.Pipeline = pipeline
compiler.Parameters.OutputType = CompilerOutputType.Library

result = compiler.Run()
print result.Errors.ToString(true)
