using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class MemberReferenceExpression : MemberReferenceExpressionImpl
	{		
		public MemberReferenceExpression()
		{
 		}
		
		public MemberReferenceExpression(Expression target) : base(target)
		{
		}
		
		public MemberReferenceExpression(antlr.Token token, Expression target) : base(token, target)
		{
		}
		
		internal MemberReferenceExpression(antlr.Token token) : base(token)
		{
		}
		
		internal MemberReferenceExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnMemberReferenceExpression(this);
		}
	}
}
