using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class ContinueStatementImpl : Statement
	{
		
		protected ContinueStatementImpl()
		{
 		}
		
		internal ContinueStatementImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal ContinueStatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
	}
}
