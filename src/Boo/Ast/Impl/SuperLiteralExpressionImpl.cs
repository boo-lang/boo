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
			SuperLiteralExpression thisNode = (SuperLiteralExpression)this;
			Expression resultingTypedNode = thisNode;
			transformer.OnSuperLiteralExpression(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
