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
			BreakStatement thisNode = (BreakStatement)this;
			Statement resultingTypedNode = thisNode;
			transformer.OnBreakStatement(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
