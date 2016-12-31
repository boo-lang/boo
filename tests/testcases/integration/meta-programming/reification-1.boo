"""
Foo.Bar(1)
Foo.Bar(2)
"""

import Boo.Lang.Environments

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem.Services

interface IFoo:
	def Bar()
	
class ImplementIFoo(AbstractVisitorCompilerStep):
	override def Run():
		Visit(CompileUnit)
		
	override def LeaveClassDefinition(node as ClassDefinition):
		countField = [|
			private _count = 1
		|]
		my(CodeReifier).ReifyInto(node, countField)
		
		node.BaseTypes.Add(CodeBuilder.CreateTypeReference(IFoo))
		bar = [|
			virtual def Bar():
				print "Foo.Bar($_count)"
				_count++
		|]
		my(CodeReifier).ReifyInto(node, bar)
				
	
module = [|
	import System
	
	class Foo:
		pass
|]

pipeline = Pipelines.CompileToMemory()
pipeline.InsertAfter(Steps.TypeInference, ImplementIFoo())

parameters = CompilerParameters(Pipeline: pipeline)
parameters.References.Add(typeof(IFoo).Assembly)

result = BooCompiler(parameters).Run(CompileUnit(module))
assert len(result.Errors) == 0, result.Errors.ToString(true)

foo as IFoo = result.GeneratedAssembly.GetType("Foo")()
foo.Bar()
foo.Bar()

