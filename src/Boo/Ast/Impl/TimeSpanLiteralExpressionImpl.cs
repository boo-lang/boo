using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class TimeSpanLiteralExpressionImpl : LiteralExpression
	{
		protected string _value;
		
		protected TimeSpanLiteralExpressionImpl()
		{
 		}
		
		protected TimeSpanLiteralExpressionImpl(string value)
		{
 			Value = value;
		}
		
		protected TimeSpanLiteralExpressionImpl(antlr.Token token, string value) : base(token)
		{
 			Value = value;
		}
		
		internal TimeSpanLiteralExpressionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal TimeSpanLiteralExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		public string Value
		{
			get
			{
				return _value;
			}
			
			set
			{
				_value = value;
			}
		}
	}
}
