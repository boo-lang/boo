using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class ExpressionStatementImpl : Statement
	{
		protected Expression _expression;
		
		protected ExpressionStatementImpl()
		{
 		}
		
		protected ExpressionStatementImpl(Expression expression)
		{
 			Expression = expression;
			LexicalInfo = expression.LexicalInfo;
		}
		
		protected ExpressionStatementImpl(antlr.Token token, Expression expression) : base(token)
		{
 			Expression = expression;
			LexicalInfo = expression.LexicalInfo;
		}
		
		internal ExpressionStatementImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal ExpressionStatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.ExpressionStatement;
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
					LexicalInfo = value.LexicalInfo;
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			ExpressionStatement thisNode = (ExpressionStatement)this;
			Statement resultingTypedNode = thisNode;
			transformer.OnExpressionStatement(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
