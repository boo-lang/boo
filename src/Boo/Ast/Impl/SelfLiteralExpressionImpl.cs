using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class SelfLiteralExpressionImpl : LiteralExpression
	{
		
		protected SelfLiteralExpressionImpl()
		{
 		}
		
		internal SelfLiteralExpressionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal SelfLiteralExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.SelfLiteralExpression;
			}
		}
	}
}
