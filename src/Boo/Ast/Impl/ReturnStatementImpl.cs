using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class ReturnStatementImpl : Statement
	{
		protected Expression _expression;
		
		protected ReturnStatementImpl()
		{
 		}
		
		protected ReturnStatementImpl(Expression expression)
		{
 			Expression = expression;
		}
		
		protected ReturnStatementImpl(antlr.Token token, Expression expression) : base(token)
		{
 			Expression = expression;
		}
		
		internal ReturnStatementImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal ReturnStatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.ReturnStatement;
			}
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
