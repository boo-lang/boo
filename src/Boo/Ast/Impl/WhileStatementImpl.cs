using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class WhileStatementImpl : Statement
	{
		protected Expression _condition;
		protected StatementCollection _statements;
		
		protected WhileStatementImpl()
		{
			_statements = new StatementCollection(this);
 		}
		
		protected WhileStatementImpl(Expression condition)
		{
			_statements = new StatementCollection(this);
 			Condition = condition;
		}
		
		protected WhileStatementImpl(antlr.Token token, Expression condition) : base(token)
		{
			_statements = new StatementCollection(this);
 			Condition = condition;
		}
		
		internal WhileStatementImpl(antlr.Token token) : base(token)
		{
			_statements = new StatementCollection(this);
 		}
		
		internal WhileStatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
			_statements = new StatementCollection(this);
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.WhileStatement;
			}
		}
		public Expression Condition
		{
			get
			{
				return _condition;
			}
			
			set
			{
				_condition = value;
				if (null != _condition)
				{
					_condition.InitializeParent(this);
				}
			}
		}
		public StatementCollection Statements
		{
			get
			{
				return _statements;
			}
			
			set
			{
				_statements = value;
				if (null != _statements)
				{
					_statements.InitializeParent(this);
				}
			}
		}
	}
}
