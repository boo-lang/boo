import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps

class EmptyVisitor(AbstractCompilerStep):
	override def Run():
		CompileUnit.Accept(DepthFirstVisitor())
	
class EmptyTransformer(AbstractCompilerStep):
	override def Run():
		CompileUnit.Accept(DepthFirstTransformer())
	
class EmptyFastVisitor(AbstractCompilerStep):
	override def Run():
		CompileUnit.Accept(FastDepthFirstVisitor())
		
class EmptyGuidedVisitor(AbstractCompilerStep):
	override def Run():
		CompileUnit.Accept(DepthFirstGuide())
		
def PipelineWithMany(count as int, stepType as System.Type):
	pipeline = CompilerPipeline()
	for i in range(count):
		pipeline.Add(stepType())
	return pipeline
	
def CreateCompilerWithMany(stepType as System.Type):
	return BooCompiler(CompilerParameters(Pipeline: PipelineWithMany(1000, stepType)))
	
def Benchmark(compiler as BooCompiler, compileUnit as CompileUnit):
	privateCompileUnit = compileUnit.CloneNode()
	sw = System.Diagnostics.Stopwatch.StartNew()
	compiler.Run(privateCompileUnit)
	sw.Stop()
	print "$(compiler.Parameters.Pipeline[-1]): $(sw.Elapsed)"
	
def Parse(fname as string):
	parser = BooCompiler(CompilerParameters(Pipeline: Pipelines.Parse()))
	parser.Parameters.Input.Add(IO.FileInput(fname))
	result = parser.Run()
	if len(result.Errors) > 0:
		print result.Errors.ToString()
		return null
	return result.CompileUnit
	
if len(argv) == 0:
	print "Usage: VisitorBencharmk <boo source file>"
	return
	
fname, = argv
print "benchmarking against $fname..."

compileUnit = Parse(fname)
if compileUnit is null: return
	
visitors = CreateCompilerWithMany(EmptyVisitor)
transformers = CreateCompilerWithMany(EmptyTransformer)
guided = CreateCompilerWithMany(EmptyGuidedVisitor)
fastVisitors = CreateCompilerWithMany(EmptyFastVisitor)
for i in range(3):
	for compiler in visitors, fastVisitors, guided, transformers:
		Benchmark compiler, compileUnit
