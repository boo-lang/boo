import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps
import System.IO

class AutoImport(AbstractVisitorCompilerStep):

	override def OnImport(node as Import):

		references = self.Parameters.References
		errors = self.Errors
		
		for reference in references:
			simpleName = @/, /.Split(reference.FullName)[0]
			return if simpleName == node.Namespace			
		
		result = compile("${node.Namespace}.boo", CompilerOutputType.Library)
		if len(result.Errors):
			for e in result.Errors:
				errors.Add(e)
		else: 			
			references.Add(result.GeneratedAssembly)

	override def Run():
		Visit(CompileUnit)

def compile(fname as string, outputType as CompilerOutputType):
	pipeline = CompileToMemory()
	pipeline.Insert(1, AutoImport())
	
	print("compiling ${fname}...")
	compiler = BooCompiler()
	compiler.Parameters.OutputType = outputType
	compiler.Parameters.Input.Add(FileInput(fname))
	compiler.Parameters.Pipeline = pipeline		
	result = compiler.Run()
	print("done.")
	return result

result = compile("client.boo", CompilerOutputType.ConsoleApplication)
if len(result.Errors):
	print(result.Errors.ToString(true))
else:
	result.GeneratedAssemblyEntryPoint.Invoke(null, (null,))
		
	
	 
