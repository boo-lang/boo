using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class ForStatement : ForStatementImpl, IMultiLineStatement
	{		
		public ForStatement()
		{
			_declarations = new DeclarationCollection(this);
			_statements = new StatementCollection(this);
 		}
		
		public ForStatement(Expression iterator) : base(iterator)
		{
		}
		
		public ForStatement(antlr.Token token, Expression iterator) : base(token, iterator)
		{
		}
		
		internal ForStatement(antlr.Token token) : base(token)
		{
		}
		
		internal ForStatement(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnForStatement(this);
		}
	}
}
