import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Ast.Visitors
 
class WithMacro(AbstractAstMacro):
	
	private class NameExpander(DepthFirstTransformer):
		
		_inst as ReferenceExpression
		
		def constructor(inst as ReferenceExpression):
			_inst = inst
			
		override def OnReferenceExpression(node as ReferenceExpression):
			// if the name of the reference begins with '_'
			// then convert the reference to a member reference
			// of the provided instance
			if node.Name.StartsWith('_'):
				// create the new member reference and set it up
				mre = MemberReferenceExpression(node.LexicalInfo)
				mre.Name = node.Name[1:]
				mre.Target = _inst.CloneNode()
				
				// replace the original reference in the AST
				// with the new member-reference
				ReplaceCurrentNode(mre)
				
	override def Expand(macro as MacroStatement) as Statement:
		assert 1 == macro.Arguments.Count
		assert macro.Arguments[0] isa ReferenceExpression
		
		inst = macro.Arguments[0] as ReferenceExpression
		
		// convert all _<ref> to inst.<ref>
		block = macro.Block		
		ne = NameExpander(inst)
		ne.Visit(block)
		return block
