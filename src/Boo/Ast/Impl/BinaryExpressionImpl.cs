using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class BinaryExpressionImpl : Expression
	{
		protected BinaryOperatorType _operator;
		protected Expression _left;
		protected Expression _right;
		
		protected BinaryExpressionImpl()
		{
 		}
		
		protected BinaryExpressionImpl(BinaryOperatorType operator_, Expression left, Expression right)
		{
 			Operator = operator_;
			Left = left;
			Right = right;
		}
		
		protected BinaryExpressionImpl(antlr.Token token, BinaryOperatorType operator_, Expression left, Expression right) : base(token)
		{
 			Operator = operator_;
			Left = left;
			Right = right;
		}
		
		internal BinaryExpressionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal BinaryExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public BinaryOperatorType Operator
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
		
		public Expression Left
		{
			get
			{
				return _left;
			}
			
			set
			{
				_left = value;
				if (null != _left)
				{
					_left.InitializeParent(this);
				}
			}
		}
		
		public Expression Right
		{
			get
			{
				return _right;
			}
			
			set
			{
				_right = value;
				if (null != _right)
				{
					_right.InitializeParent(this);
				}
			}
		}
	}
}
