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
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.ContinueStatement;
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			Statement resultingTypedNode;
			transformer.OnContinueStatement((ContinueStatement)this, out resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
