using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class RetryStatementImpl : Statement
	{
		
		protected RetryStatementImpl()
		{
 		}
		
		internal RetryStatementImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal RetryStatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
	}
}
