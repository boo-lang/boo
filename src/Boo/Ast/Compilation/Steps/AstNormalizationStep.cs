using System;
using Boo.Ast;
using Boo.Ast.Compilation;

namespace Boo.Ast.Compilation.Steps
{
	public class AstNormalizationStep : AbstractTransformerCompilerStep
	{
		public override void Run()
		{
			foreach (Module module in CompileUnit.Modules)
			{
				Switch(module.Globals.Statements);
				Switch(module.Members);
			}
		}
		
		public override void LeaveExpressionStatement(ExpressionStatement node, ref Statement resultingNode)
		{
			if (null != node.Modifier)
			{
				switch (node.Modifier.Type)
				{
					case StatementModifierType.If:
					{	
						IfStatement stmt = new IfStatement(node.Modifier);
						stmt.Expression = node.Modifier.Condition;
						stmt.TrueBlock = new Block(node.Modifier);
						stmt.TrueBlock.Statements.Add(node);						
						node.Modifier = null;
						
						resultingNode = stmt;
						
						break;
					}
						
					default:
					{							
						Errors.NotImplemented(node, "only if supported");
						break;
					}
				}
			}
		}
	}
}
