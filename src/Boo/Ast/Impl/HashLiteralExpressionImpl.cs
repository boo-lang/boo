using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class HashLiteralExpressionImpl : LiteralExpression
	{
		protected ExpressionPairCollection _items;
		
		protected HashLiteralExpressionImpl()
		{
			_items = new ExpressionPairCollection(this);
 		}
		
		internal HashLiteralExpressionImpl(antlr.Token token) : base(token)
		{
			_items = new ExpressionPairCollection(this);
 		}
		
		internal HashLiteralExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
			_items = new ExpressionPairCollection(this);
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.HashLiteralExpression;
			}
		}
		public ExpressionPairCollection Items
		{
			get
			{
				return _items;
			}
			
			set
			{
				
				if (_items != value)
				{
					_items = value;
					if (null != _items)
					{
						_items.InitializeParent(this);
					}
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			HashLiteralExpression thisNode = (HashLiteralExpression)this;
			Expression resultingTypedNode = thisNode;
			transformer.OnHashLiteralExpression(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
