import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Ast.Visitors

class AliasMacro(AbstractAstMacro):

	static final Usage = "Usage: alias <expression> as <name>"
				
	override def Expand(macro as MacroStatement):
		if not CheckUsage(macro):
			Errors.Add(
				CompilerErrorFactory.CustomError(macro.LexicalInfo, Usage))
			return null
		
		argument as TryCastExpression = macro.Arguments[0]
		reference = ReferenceExpression(Name: argument.Type.ToString())		
		macro.ParentNode.ReplaceNodes(reference, argument.Target)
		
	def CheckUsage(macro as MacroStatement):
		if len(macro.Block.Statements) > 0: return false
		if len(macro.Arguments) != 1: return false
		expression = macro.Arguments[0] as TryCastExpression
		if expression is null: return false
		return expression.Type isa SimpleTypeReference
