using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class RetryStatement : RetryStatementImpl
	{		
		public RetryStatement()
		{
 		}
		
		internal RetryStatement(antlr.Token token) : base(token)
		{
		}
		
		internal RetryStatement(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnRetryStatement(this);
		}
	}
}
