using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class DeclarationStatement : DeclarationStatementImpl
	{		
		public DeclarationStatement()
		{
 		}
		
		public DeclarationStatement(Declaration declaration, Expression initializer) : base(declaration, initializer)
		{
		}
		
		public DeclarationStatement(antlr.Token token, Declaration declaration, Expression initializer) : base(token, declaration, initializer)
		{
		}
		
		internal DeclarationStatement(antlr.Token token) : base(token)
		{
		}
		
		internal DeclarationStatement(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnDeclarationStatement(this);
		}
	}
}
