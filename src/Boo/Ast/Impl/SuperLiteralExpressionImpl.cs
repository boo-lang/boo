using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class SuperLiteralExpressionImpl : LiteralExpression
	{
		
		protected SuperLiteralExpressionImpl()
		{
 		}
		
		internal SuperLiteralExpressionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal SuperLiteralExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.SuperLiteralExpression;
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			Expression resultingTypedNode;
			transformer.OnSuperLiteralExpression((SuperLiteralExpression)this, out resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
