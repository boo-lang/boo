using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class TimeSpanLiteralExpression : TimeSpanLiteralExpressionImpl
	{		
		public TimeSpanLiteralExpression()
		{
 		}
		
		public TimeSpanLiteralExpression(string value) : base(value)
		{
		}
		
		public TimeSpanLiteralExpression(antlr.Token token, string value) : base(token, value)
		{
		}
		
		internal TimeSpanLiteralExpression(antlr.Token token) : base(token)
		{
		}
		
		internal TimeSpanLiteralExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnTimeSpanLiteralExpression(this);
		}
	}
}
