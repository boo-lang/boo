using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class LiteralExpressionImpl : Expression
	{
		
		protected LiteralExpressionImpl()
		{
 		}
		
		internal LiteralExpressionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal LiteralExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.LiteralExpression;
			}
		}
	}
}
