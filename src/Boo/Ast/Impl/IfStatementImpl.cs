using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class IfStatementImpl : Statement
	{
		protected Expression _expression;
		protected Block _trueBlock;
		protected Block _falseBlock;
		
		protected IfStatementImpl()
		{
 		}
		
		protected IfStatementImpl(Expression expression, Block trueBlock, Block falseBlock)
		{
 			Expression = expression;
			TrueBlock = trueBlock;
			FalseBlock = falseBlock;
		}
		
		protected IfStatementImpl(antlr.Token token, Expression expression, Block trueBlock, Block falseBlock) : base(token)
		{
 			Expression = expression;
			TrueBlock = trueBlock;
			FalseBlock = falseBlock;
		}
		
		internal IfStatementImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal IfStatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
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
		
		public Block TrueBlock
		{
			get
			{
				return _trueBlock;
			}
			
			set
			{
				_trueBlock = value;
				if (null != _trueBlock)
				{
					_trueBlock.InitializeParent(this);
				}
			}
		}
		
		public Block FalseBlock
		{
			get
			{
				return _falseBlock;
			}
			
			set
			{
				_falseBlock = value;
				if (null != _falseBlock)
				{
					_falseBlock.InitializeParent(this);
				}
			}
		}
	}
}
