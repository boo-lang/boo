using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class GivenStatementImpl : Statement
	{
		protected Expression _expression;
		protected WhenClauseCollection _whenClauses;
		protected Block _otherwiseBlock;
		
		protected GivenStatementImpl()
		{
			_whenClauses = new WhenClauseCollection(this);
 		}
		
		protected GivenStatementImpl(Expression expression, Block otherwiseBlock)
		{
			_whenClauses = new WhenClauseCollection(this);
 			Expression = expression;
			OtherwiseBlock = otherwiseBlock;
		}
		
		protected GivenStatementImpl(antlr.Token token, Expression expression, Block otherwiseBlock) : base(token)
		{
			_whenClauses = new WhenClauseCollection(this);
 			Expression = expression;
			OtherwiseBlock = otherwiseBlock;
		}
		
		internal GivenStatementImpl(antlr.Token token) : base(token)
		{
			_whenClauses = new WhenClauseCollection(this);
 		}
		
		internal GivenStatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
			_whenClauses = new WhenClauseCollection(this);
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
		
		public WhenClauseCollection WhenClauses
		{
			get
			{
				return _whenClauses;
			}
			
			set
			{
				_whenClauses = value;
				if (null != _whenClauses)
				{
					_whenClauses.InitializeParent(this);
				}
			}
		}
		
		public Block OtherwiseBlock
		{
			get
			{
				return _otherwiseBlock;
			}
			
			set
			{
				_otherwiseBlock = value;
				if (null != _otherwiseBlock)
				{
					_otherwiseBlock.InitializeParent(this);
				}
			}
		}
	}
}
