using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class BoolLiteralExpression : BoolLiteralExpressionImpl
	{		
		public BoolLiteralExpression()
		{
 		}
		
		public BoolLiteralExpression(bool value) : base(value)
		{
		}
		
		public BoolLiteralExpression(antlr.Token token, bool value) : base(token, value)
		{
		}
		
		internal BoolLiteralExpression(antlr.Token token) : base(token)
		{
		}
		
		internal BoolLiteralExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnBoolLiteralExpression(this);
		}
	}
}
