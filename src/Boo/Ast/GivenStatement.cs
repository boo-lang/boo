using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class GivenStatement : GivenStatementImpl
	{		
		public GivenStatement()
		{
			_whenClauses = new WhenClauseCollection(this);
 		}
		
		public GivenStatement(Expression expression, Block otherwiseBlock) : base(expression, otherwiseBlock)
		{
		}
		
		public GivenStatement(antlr.Token token, Expression expression, Block otherwiseBlock) : base(token, expression, otherwiseBlock)
		{
		}
		
		internal GivenStatement(antlr.Token token) : base(token)
		{
		}
		
		internal GivenStatement(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnGivenStatement(this);
		}
	}
}
