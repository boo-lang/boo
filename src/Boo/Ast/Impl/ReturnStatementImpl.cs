using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class ReturnStatementImpl : Statement
	{
		protected Expression _expression;
		
		protected ReturnStatementImpl()
		{
 		}
		
		protected ReturnStatementImpl(Expression expression)
		{
 			Expression = expression;
		}
		
		protected ReturnStatementImpl(antlr.Token token, Expression expression) : base(token)
		{
 			Expression = expression;
		}
		
		internal ReturnStatementImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal ReturnStatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.ReturnStatement;
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
			ReturnStatement thisNode = (ReturnStatement)this;
			Statement resultingTypedNode = thisNode;
			transformer.OnReturnStatement(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
