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
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.IfStatement;
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
				
				if (_expression != value)
				{
					_expression = value;
					if (null != _expression)
					{
						_expression.InitializeParent(this);
					}
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
				
				if (_trueBlock != value)
				{
					_trueBlock = value;
					if (null != _trueBlock)
					{
						_trueBlock.InitializeParent(this);
					}
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
				
				if (_falseBlock != value)
				{
					_falseBlock = value;
					if (null != _falseBlock)
					{
						_falseBlock.InitializeParent(this);
					}
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			IfStatement thisNode = (IfStatement)this;
			Statement resultingTypedNode = thisNode;
			transformer.OnIfStatement(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
