using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class ExpressionPairImpl : Node
	{
		protected Expression _first;
		protected Expression _second;
		
		protected ExpressionPairImpl()
		{
 		}
		
		protected ExpressionPairImpl(Expression first, Expression second)
		{
 			First = first;
			Second = second;
		}
		
		protected ExpressionPairImpl(antlr.Token token, Expression first, Expression second) : base(token)
		{
 			First = first;
			Second = second;
		}
		
		internal ExpressionPairImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal ExpressionPairImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.ExpressionPair;
			}
		}
		public Expression First
		{
			get
			{
				return _first;
			}
			
			set
			{
				
				if (_first != value)
				{
					_first = value;
					if (null != _first)
					{
						_first.InitializeParent(this);
					}
				}
			}
		}
		public Expression Second
		{
			get
			{
				return _second;
			}
			
			set
			{
				
				if (_second != value)
				{
					_second = value;
					if (null != _second)
					{
						_second.InitializeParent(this);
					}
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			ExpressionPair thisNode = (ExpressionPair)this;
			ExpressionPair resultingTypedNode = thisNode;
			transformer.OnExpressionPair(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
