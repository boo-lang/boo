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
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.RetryStatement;
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			Statement resultingTypedNode;
			transformer.OnRetryStatement((RetryStatement)this, out resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
