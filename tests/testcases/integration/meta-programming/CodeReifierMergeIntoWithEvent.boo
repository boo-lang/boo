"""
Foo.Trigger(1)
Foo.Trigger(2)
Bar.Trigger(1)
Bar.Trigger(2)
"""

import Boo.Lang.Environments

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem.Services

class FooEventArgs(System.EventArgs):
	public Message as string
	
interface IFoo:
	event Triggered as System.EventHandler of FooEventArgs
	def Trigger()
	
class ImplementIFoo(AbstractVisitorCompilerStep):
	
	override def Run():
		Visit(CompileUnit)
		
	override def LeaveClassDefinition(node as ClassDefinition):
		impl = [|
			class _($IFoo):
					
				_count = 1
				
				event Triggered as System.EventHandler of $FooEventArgs
	
				def Trigger():
					Triggered(self, FooEventArgs(Message: "$(GetType().Name).Trigger($_count)"))
					++_count
		|]
		my(CodeReifier).MergeInto(node, impl)
		
def test(foo as IFoo):
	foo.Triggered += do (sender, args as FooEventArgs):
		print args.Message
	foo.Trigger()
	foo.Trigger()
	
module = [|
	import System
	
	class Foo:
		pass
		
	class Bar:
		pass
|]

pipeline = Pipelines.CompileToMemory()
pipeline.InsertAfter(Steps.TypeInference, ImplementIFoo())

parameters = CompilerParameters(Pipeline: pipeline)
parameters.References.Add(typeof(IFoo).Assembly)

result = BooCompiler(parameters).Run(CompileUnit(module))
assert len(result.Errors) == 0, result.Errors.ToString(true)

assembly = result.GeneratedAssembly
test assembly.GetType("Foo")()
test assembly.GetType("Bar")()
	

