using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class NullLiteralExpressionImpl : LiteralExpression
	{
		
		protected NullLiteralExpressionImpl()
		{
 		}
		
		internal NullLiteralExpressionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal NullLiteralExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.NullLiteralExpression;
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			NullLiteralExpression thisNode = (NullLiteralExpression)this;
			Expression resultingTypedNode = thisNode;
			transformer.OnNullLiteralExpression(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
