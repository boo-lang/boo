using System;
using Boo.Ast;
using Boo.Ast.Compilation;

namespace Boo.Ast.Compilation.Steps
{
	public class AstNormalizationStep : AbstractCompilerStep
	{
		public override void Run()
		{
			CompileUnit.Switch(this);
		}
		
		public override void OnExpressionStatement(ExpressionStatement node)
		{
			if (null != node.Modifier)
			{
				switch (node.Modifier.Type)
				{
					case StatementModifierType.If:
					{						
						IMultiLineStatement parent = (IMultiLineStatement)node.ParentNode;
						
						IfStatement stmt = new IfStatement(node.Modifier);
						stmt.Expression = node.Modifier.Condition;
						stmt.TrueBlock = new Block(node.Modifier);
						stmt.TrueBlock.Statements.Add(node);						
						
						parent.Statements.Replace(node, stmt);
						
						node.Modifier = null;
						
						break;
					}
						
					default:
					{							
						throw new NotImplementedException("only if at this time!");
					}
				}
			}
		}
	}
}
