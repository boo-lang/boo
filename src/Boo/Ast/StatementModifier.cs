using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class StatementModifier : StatementModifierImpl
	{		
		public StatementModifier()
		{
 		}
		
		public StatementModifier(StatementModifierType type, Expression condition) : base(type, condition)
		{
		}
		
		public StatementModifier(antlr.Token token, StatementModifierType type, Expression condition) : base(token, type, condition)
		{
		}
		
		internal StatementModifier(antlr.Token token) : base(token)
		{
		}
		
		internal StatementModifier(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnStatementModifier(this);
		}
	}
}
