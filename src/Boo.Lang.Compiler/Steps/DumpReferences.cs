namespace Boo.Lang.Compiler.Steps
{
	using System;
	using Boo.Lang.Compiler.Ast;
	
	public class DumpReferences : AbstractVisitorCompilerStep
	{	
		override public void Run()
		{
			Visit(CompileUnit);
		}
		
		override public void OnReferenceExpression(ReferenceExpression node)
		{
			Console.WriteLine("{0}: '{1}': {2}", node.LexicalInfo, node.Name, node.Entity);
			Console.WriteLine("{0}: '{1}': {2}", node.LexicalInfo, node.Name, node.ExpressionType);
		}
		
		override public void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			OnReferenceExpression(node);
		}
	}
}
