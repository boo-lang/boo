using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class ReturnStatement : ReturnStatementImpl
	{		
		public ReturnStatement()
		{
 		}
		
		public ReturnStatement(Expression expression) : base(expression)
		{
		}
		
		public ReturnStatement(antlr.Token token, Expression expression) : base(token, expression)
		{
		}
		
		internal ReturnStatement(antlr.Token token) : base(token)
		{
		}
		
		internal ReturnStatement(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnReturnStatement(this);
		}
	}
}
