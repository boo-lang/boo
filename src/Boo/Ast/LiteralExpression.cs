using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[System.Xml.Serialization.XmlInclude(typeof(StringLiteralExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(TimeSpanLiteralExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(IntegerLiteralExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(NullLiteralExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(SelfLiteralExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(SuperLiteralExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(BoolLiteralExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(RELiteralExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(HashLiteralExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(ListLiteralExpression))]
	[Serializable]
	public class LiteralExpression : LiteralExpressionImpl
	{		
		public LiteralExpression()
		{
 		}
		
		internal LiteralExpression(antlr.Token token) : base(token)
		{
		}
		
		internal LiteralExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnLiteralExpression(this);
		}
	}
}
