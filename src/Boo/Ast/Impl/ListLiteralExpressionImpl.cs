using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class ListLiteralExpressionImpl : LiteralExpression
	{
		protected ExpressionCollection _items;
		
		protected ListLiteralExpressionImpl()
		{
			_items = new ExpressionCollection(this);
 		}
		
		internal ListLiteralExpressionImpl(antlr.Token token) : base(token)
		{
			_items = new ExpressionCollection(this);
 		}
		
		internal ListLiteralExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
			_items = new ExpressionCollection(this);
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.ListLiteralExpression;
			}
		}
		public ExpressionCollection Items
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
			ListLiteralExpression thisNode = (ListLiteralExpression)this;
			Expression resultingTypedNode = thisNode;
			transformer.OnListLiteralExpression(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
