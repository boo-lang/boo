using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class AsExpression : AsExpressionImpl
	{		
		public AsExpression()
		{
 		}
		
		public AsExpression(Expression target, TypeReference type) : base(target, type)
		{
		}
		
		public AsExpression(antlr.Token token, Expression target, TypeReference type) : base(token, target, type)
		{
		}
		
		internal AsExpression(antlr.Token token) : base(token)
		{
		}
		
		internal AsExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnAsExpression(this);
		}
	}
}
