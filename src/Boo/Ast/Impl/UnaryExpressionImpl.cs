using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class UnaryExpressionImpl : Expression
	{
		protected UnaryOperatorType _operator;
		protected Expression _operand;
		
		protected UnaryExpressionImpl()
		{
 		}
		
		protected UnaryExpressionImpl(UnaryOperatorType operator_, Expression operand)
		{
 			Operator = operator_;
			Operand = operand;
		}
		
		protected UnaryExpressionImpl(antlr.Token token, UnaryOperatorType operator_, Expression operand) : base(token)
		{
 			Operator = operator_;
			Operand = operand;
		}
		
		internal UnaryExpressionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal UnaryExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.UnaryExpression;
			}
		}
		public UnaryOperatorType Operator
		{
			get
			{
				return _operator;
			}
			
			set
			{
				_operator = value;
			}
		}
		public Expression Operand
		{
			get
			{
				return _operand;
			}
			
			set
			{
				_operand = value;
				if (null != _operand)
				{
					_operand.InitializeParent(this);
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			Expression resultingTypedNode;
			transformer.OnUnaryExpression((UnaryExpression)this, out resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
