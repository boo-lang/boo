using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class BreakStatementImpl : Statement
	{
		
		protected BreakStatementImpl()
		{
 		}
		
		internal BreakStatementImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal BreakStatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
	}
}
