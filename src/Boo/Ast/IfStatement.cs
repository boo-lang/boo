using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class IfStatement : IfStatementImpl
	{		
		public IfStatement()
		{
 		}
		
		public IfStatement(Expression expression, Block trueBlock, Block falseBlock) : base(expression, trueBlock, falseBlock)
		{
		}
		
		public IfStatement(antlr.Token token, Expression expression, Block trueBlock, Block falseBlock) : base(token, expression, trueBlock, falseBlock)
		{
		}
		
		internal IfStatement(antlr.Token token) : base(token)
		{
		}
		
		internal IfStatement(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnIfStatement(this);
		}
	}
}
