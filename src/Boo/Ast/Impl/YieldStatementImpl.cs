using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class YieldStatementImpl : Statement
	{
		protected Expression _expression;
		
		protected YieldStatementImpl()
		{
 		}
		
		protected YieldStatementImpl(Expression expression)
		{
 			Expression = expression;
		}
		
		protected YieldStatementImpl(antlr.Token token, Expression expression) : base(token)
		{
 			Expression = expression;
		}
		
		internal YieldStatementImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal YieldStatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.YieldStatement;
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
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			YieldStatement thisNode = (YieldStatement)this;
			Statement resultingTypedNode = thisNode;
			transformer.OnYieldStatement(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
