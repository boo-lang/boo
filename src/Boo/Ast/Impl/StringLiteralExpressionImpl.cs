using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class StringLiteralExpressionImpl : LiteralExpression
	{
		protected string _value;
		
		protected StringLiteralExpressionImpl()
		{
 		}
		
		protected StringLiteralExpressionImpl(string value)
		{
 			Value = value;
		}
		
		protected StringLiteralExpressionImpl(antlr.Token token, string value) : base(token)
		{
 			Value = value;
		}
		
		internal StringLiteralExpressionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal StringLiteralExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.StringLiteralExpression;
			}
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
