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
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.WhenClause;
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
				
				if (_condition != value)
				{
					_condition = value;
					if (null != _condition)
					{
						_condition.InitializeParent(this);
					}
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			WhenClause thisNode = (WhenClause)this;
			WhenClause resultingTypedNode = thisNode;
			transformer.OnWhenClause(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
