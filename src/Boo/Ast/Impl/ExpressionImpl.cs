using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class ExpressionImpl : Node
	{
		
		protected ExpressionImpl()
		{
 		}
		
		internal ExpressionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal ExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
	}
}
