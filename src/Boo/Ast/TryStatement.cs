using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class TryStatement : TryStatementImpl
	{		
		public TryStatement()
		{
			ProtectedBlock = new Block();
			_exceptionHandlers = new ExceptionHandlerCollection(this);
 		}
		
		public TryStatement(Block successBlock, Block ensureBlock) : base(successBlock, ensureBlock)
		{
		}
		
		public TryStatement(antlr.Token token, Block successBlock, Block ensureBlock) : base(token, successBlock, ensureBlock)
		{
		}
		
		internal TryStatement(antlr.Token token) : base(token)
		{
		}
		
		internal TryStatement(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnTryStatement(this);
		}
	}
}
