using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class YieldStatementImpl : Statement
	{
		protected Expression _expression;
		
		protected YieldStatementImpl()
		{
 		}
		
		protected YieldStatementImpl(Expression expression)
		{
 			Expression = expression;
		}
		
		protected YieldStatementImpl(antlr.Token token, Expression expression) : base(token)
		{
 			Expression = expression;
		}
		
		internal YieldStatementImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal YieldStatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
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
			}
		}
	}
}
