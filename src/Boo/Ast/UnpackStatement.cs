using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class UnpackStatement : UnpackStatementImpl
	{		
		public UnpackStatement()
		{
			_declarations = new DeclarationCollection(this);
 		}
		
		public UnpackStatement(Expression expression) : base(expression)
		{
		}
		
		public UnpackStatement(antlr.Token token, Expression expression) : base(token, expression)
		{
		}
		
		internal UnpackStatement(antlr.Token token) : base(token)
		{
		}
		
		internal UnpackStatement(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnUnpackStatement(this);
		}
	}
}
