"""
Foo.Bar(1)
Foo.Bar(3)
"""

import Boo.Lang.Environments

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem.Services

interface IFoo:
	Bar as string:
		get
	Baz as int:
		get
	
class ImplementIFoo(AbstractVisitorCompilerStep):
	
	override def Run():
		Visit(CompileUnit)
		
	override def LeaveClassDefinition(node as ClassDefinition):
		impl = [|
			class _($IFoo):
					
				private _count = 1
	
				Bar:
					get: return "Foo.Bar($_count)"
				
				Baz:
					get: return _count++
		|]
		my(CodeReifier).MergeInto(node, impl)
				
	
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
print foo.Bar
assert foo.Baz == 1
assert foo.Baz == 2
print foo.Bar

