using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class AssertStatementImpl : Statement
	{
		protected Expression _condition;
		protected Expression _message;
		
		protected AssertStatementImpl()
		{
 		}
		
		protected AssertStatementImpl(Expression condition, Expression message)
		{
 			Condition = condition;
			Message = message;
		}
		
		protected AssertStatementImpl(antlr.Token token, Expression condition, Expression message) : base(token)
		{
 			Condition = condition;
			Message = message;
		}
		
		internal AssertStatementImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal AssertStatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.AssertStatement;
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
		public Expression Message
		{
			get
			{
				return _message;
			}
			
			set
			{
				_message = value;
				if (null != _message)
				{
					_message.InitializeParent(this);
				}
			}
		}
	}
}
