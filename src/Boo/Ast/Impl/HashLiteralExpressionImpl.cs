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
				_items = value;
				if (null != _items)
				{
					_items.InitializeParent(this);
				}
			}
		}
	}
}
