using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[System.Xml.Serialization.XmlInclude(typeof(TupleLiteralExpression))]
	[Serializable]
	public class ListLiteralExpression : ListLiteralExpressionImpl
	{		
		public ListLiteralExpression()
		{
			_items = new ExpressionCollection(this);
 		}
		
		internal ListLiteralExpression(antlr.Token token) : base(token)
		{
		}
		
		internal ListLiteralExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnListLiteralExpression(this);
		}
	}
}
