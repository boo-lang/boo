using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class WhenClauseImpl : Block
	{
		protected Expression _condition;
		
		protected WhenClauseImpl()
		{
 		}
		
		protected WhenClauseImpl(Expression condition)
		{
 			Condition = condition;
		}
		
		protected WhenClauseImpl(antlr.Token token, Expression condition) : base(token)
		{
 			Condition = condition;
		}
		
		internal WhenClauseImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal WhenClauseImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
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
	}
}
