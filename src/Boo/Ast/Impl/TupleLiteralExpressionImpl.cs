using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class TupleLiteralExpressionImpl : ListLiteralExpression
	{
		
		protected TupleLiteralExpressionImpl()
		{
 		}
		
		internal TupleLiteralExpressionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal TupleLiteralExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.TupleLiteralExpression;
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			TupleLiteralExpression thisNode = (TupleLiteralExpression)this;
			Expression resultingTypedNode = thisNode;
			transformer.OnTupleLiteralExpression(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
