using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class RELiteralExpressionImpl : LiteralExpression
	{
		protected string _value;
		
		protected RELiteralExpressionImpl()
		{
 		}
		
		protected RELiteralExpressionImpl(string value)
		{
 			Value = value;
		}
		
		protected RELiteralExpressionImpl(antlr.Token token, string value) : base(token)
		{
 			Value = value;
		}
		
		internal RELiteralExpressionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal RELiteralExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
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
