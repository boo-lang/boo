using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class BoolLiteralExpressionImpl : LiteralExpression
	{
		protected bool _value;
		
		protected BoolLiteralExpressionImpl()
		{
 		}
		
		protected BoolLiteralExpressionImpl(bool value)
		{
 			Value = value;
		}
		
		protected BoolLiteralExpressionImpl(antlr.Token token, bool value) : base(token)
		{
 			Value = value;
		}
		
		internal BoolLiteralExpressionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal BoolLiteralExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		public bool Value
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
