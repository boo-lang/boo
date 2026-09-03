namespace BooCompiler.Tests

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

class AllParametersRequiredAttribute(AbstractAstAttribute):
	def constructor():
		pass

	override public def Apply(node as Node):
		m = node as Method
		if m is null:
			raise ApplicationException("This attribute can only be applied to methods.")

		for pd as ParameterDeclaration in m.Parameters:
			pd.Attributes.Add(Boo.Lang.Compiler.Ast.Attribute("required"))
