using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class SlicingExpression : SlicingExpressionImpl
	{		
		public SlicingExpression()
		{
 		}
		
		public SlicingExpression(Expression target, Expression begin, Expression end, Expression step) : base(target, begin, end, step)
		{
		}
		
		public SlicingExpression(antlr.Token token, Expression target, Expression begin, Expression end, Expression step) : base(token, target, begin, end, step)
		{
		}
		
		internal SlicingExpression(antlr.Token token) : base(token)
		{
		}
		
		internal SlicingExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnSlicingExpression(this);
		}
	}
}
