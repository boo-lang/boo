using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class ExpressionStatementImpl : Statement
	{
		protected Expression _expression;
		
		protected ExpressionStatementImpl()
		{
 		}
		
		protected ExpressionStatementImpl(Expression expression)
		{
 			Expression = expression;
			LexicalInfo = expression.LexicalInfo;
		}
		
		protected ExpressionStatementImpl(antlr.Token token, Expression expression) : base(token)
		{
 			Expression = expression;
			LexicalInfo = expression.LexicalInfo;
		}
		
		internal ExpressionStatementImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal ExpressionStatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		public Expression Expression
		{
			get
			{
				return _expression;
			}
			
			set
			{
				_expression = value;
				if (null != _expression)
				{
					_expression.InitializeParent(this);
				}
				LexicalInfo = value.LexicalInfo;
			}
		}
	}
}
