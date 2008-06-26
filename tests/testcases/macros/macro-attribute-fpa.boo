"""
public class Iteration3(object):

	public def constructor():
		super()

public class Iteration2(object):

	public def constructor():
		super()

public class Iteration1(object):

	public def constructor():
		super()
"""

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

class ExpandAttribute(AbstractAstAttribute):

	_iteration as IntegerLiteralExpression
	
	def constructor(iteration as IntegerLiteralExpression):
		_iteration = iteration

	override def Apply(node as Node):
		
		method as Method = node
		method.Body = [|
			$(method.Body)
			expand $_iteration
		|]

macro expand:

	iteration as IntegerLiteralExpression = expand.Arguments[0]
	if iteration.Value < 1: return null
	
	name = "Iteration${iteration.Value}"
	iteration.Value -= 1

	klass = [|
		class $name:
			[expand($iteration)] def constructor():
				pass
	|]
			
	module as Module = expand.GetAncestor(NodeType.Module)
	module.Members.Add(klass)
	
	return null
	
code = [|

	namespace Test

	expand 3
|]

compile_(code, typeof(ExpandAttribute).Assembly)
for type in code.Members:
	continue unless type.Name.StartsWith("Iteration")
	print type.ToCodeString()

