using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class YieldStatement : YieldStatementImpl
	{		
		public YieldStatement()
		{
 		}
		
		public YieldStatement(Expression expression) : base(expression)
		{
		}
		
		public YieldStatement(antlr.Token token, Expression expression) : base(token, expression)
		{
		}
		
		internal YieldStatement(antlr.Token token) : base(token)
		{
		}
		
		internal YieldStatement(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnYieldStatement(this);
		}
	}
}
