using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class MethodInvocationExpression : MethodInvocationExpressionImpl
	{		
		public MethodInvocationExpression()
		{
			_arguments = new ExpressionCollection(this);
			_namedArguments = new ExpressionPairCollection(this);
 		}
		
		public MethodInvocationExpression(Expression target) : base(target)
		{
		}
		
		public MethodInvocationExpression(antlr.Token token, Expression target) : base(token, target)
		{
		}
		
		internal MethodInvocationExpression(antlr.Token token) : base(token)
		{
		}
		
		internal MethodInvocationExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnMethodInvocationExpression(this);
		}
	}
}
