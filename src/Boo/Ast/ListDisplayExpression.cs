using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class ListDisplayExpression : ListDisplayExpressionImpl
	{		
		public ListDisplayExpression()
		{
			_declarations = new DeclarationCollection(this);
 		}
		
		public ListDisplayExpression(Expression expression, Expression iterator, StatementModifier filter) : base(expression, iterator, filter)
		{
		}
		
		public ListDisplayExpression(antlr.Token token, Expression expression, Expression iterator, StatementModifier filter) : base(token, expression, iterator, filter)
		{
		}
		
		internal ListDisplayExpression(antlr.Token token) : base(token)
		{
		}
		
		internal ListDisplayExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnListDisplayExpression(this);
		}
	}
}
