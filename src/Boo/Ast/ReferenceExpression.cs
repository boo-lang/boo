using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[System.Xml.Serialization.XmlInclude(typeof(MemberReferenceExpression))]
	[Serializable]
	public class ReferenceExpression : ReferenceExpressionImpl
	{		
		public ReferenceExpression()
		{
 		}
		
		public ReferenceExpression(string name) : base(name)
		{
		}
		
		public ReferenceExpression(antlr.Token token, string name) : base(token, name)
		{
		}
		
		internal ReferenceExpression(antlr.Token token) : base(token)
		{
		}
		
		internal ReferenceExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnReferenceExpression(this);
		}
	}
}
