import Compiler
import Compiler.Ast
import Compiler.MetaProgramming

class DumpNodeTypeAttribute(AbstractAstAttribute):
	override def Apply(node as Node):
		Context['TheNode'] = node
		
module = [|
	namespace test
	[module: DumpNodeType]
|]
result = compile_(module, typeof(DumpNodeTypeAttribute).Assembly)
assert module is result['TheNode']