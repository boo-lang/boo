using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[System.Xml.Serialization.XmlInclude(typeof(OmittedExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(MethodInvocationExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(UnaryExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(BinaryExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(TernaryExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(ReferenceExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(LiteralExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(StringFormattingExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(ListDisplayExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(SlicingExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(AsExpression))]
	[Serializable]
	public abstract class Expression : ExpressionImpl
	{		
		public Expression()
		{
 		}
		
		internal Expression(antlr.Token token) : base(token)
		{
		}
		
		internal Expression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
	}
}
