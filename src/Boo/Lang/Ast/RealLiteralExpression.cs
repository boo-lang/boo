using System;
using Boo.Lang.Ast.Impl;

namespace Boo.Lang.Ast
{
	[Serializable]
	public class RealLiteralExpression : RealLiteralExpressionImpl
	{		
		public RealLiteralExpression()
		{
 		}
		
		public RealLiteralExpression(double value) : base(value)
		{
		}
		
		public RealLiteralExpression(LexicalInfo lexicalInfo, double value) : base(lexicalInfo, value)
		{
		}
		
		public RealLiteralExpression(LexicalInfo lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnRealLiteralExpression(this);
		}
	}
}
