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
	}
}
