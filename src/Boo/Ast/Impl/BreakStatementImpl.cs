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
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.BreakStatement;
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			Statement resultingTypedNode;
			transformer.OnBreakStatement((BreakStatement)this, out resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
