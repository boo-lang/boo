namespace Boo.Lang.Extensions

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

class MacroMacro(AbstractAstMacro):

	override def Expand(macro as MacroStatement):
		if macro.Arguments.Count != 1 or macro.Arguments[0].NodeType != NodeType.ReferenceExpression:
			Errors.Add(
				CompilerErrorFactory.CustomError(macro.LexicalInfo, "macro <reference>"))
			return null
		
		EnclosingModule(macro).Members.Add(CreateMacroType(macro))
		
	def PascalCase(name as string):
		return char.ToUpper(name[0]) + name[1:]

	private def CreateMacroType(macro as MacroStatement):
		macroName = (macro.Arguments[0] as ReferenceExpression).Name

		klass = [|
			class $(PascalCase(macroName) + "Macro")(Boo.Lang.Compiler.LexicalInfoPreservingMacro):
				override protected def ExpandImpl($macroName as Boo.Lang.Compiler.Ast.MacroStatement):
					$(macro.Block)
		|]
		klass.LexicalInfo = macro.LexicalInfo
		return klass

	private def EnclosingModule(macro as Node) as Module:
		return macro.GetAncestor(NodeType.Module)
