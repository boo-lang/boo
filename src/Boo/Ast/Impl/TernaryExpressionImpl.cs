using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class TernaryExpressionImpl : Expression
	{
		protected Expression _condition;
		protected Expression _trueExpression;
		protected Expression _falseExpression;
		
		protected TernaryExpressionImpl()
		{
 		}
		
		protected TernaryExpressionImpl(Expression condition, Expression trueExpression, Expression falseExpression)
		{
 			Condition = condition;
			TrueExpression = trueExpression;
			FalseExpression = falseExpression;
		}
		
		protected TernaryExpressionImpl(antlr.Token token, Expression condition, Expression trueExpression, Expression falseExpression) : base(token)
		{
 			Condition = condition;
			TrueExpression = trueExpression;
			FalseExpression = falseExpression;
		}
		
		internal TernaryExpressionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal TernaryExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.TernaryExpression;
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
		public Expression TrueExpression
		{
			get
			{
				return _trueExpression;
			}
			
			set
			{
				
				if (_trueExpression != value)
				{
					_trueExpression = value;
					if (null != _trueExpression)
					{
						_trueExpression.InitializeParent(this);
					}
				}
			}
		}
		public Expression FalseExpression
		{
			get
			{
				return _falseExpression;
			}
			
			set
			{
				
				if (_falseExpression != value)
				{
					_falseExpression = value;
					if (null != _falseExpression)
					{
						_falseExpression.InitializeParent(this);
					}
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			TernaryExpression thisNode = (TernaryExpression)this;
			Expression resultingTypedNode = thisNode;
			transformer.OnTernaryExpression(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
