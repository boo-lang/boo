using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class EnumMember : EnumMemberImpl
	{		
		public EnumMember()
		{
 		}
		
		public EnumMember(IntegerLiteralExpression initializer) : base(initializer)
		{
		}
		
		public EnumMember(antlr.Token token, IntegerLiteralExpression initializer) : base(token, initializer)
		{
		}
		
		internal EnumMember(antlr.Token token) : base(token)
		{
		}
		
		internal EnumMember(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnEnumMember(this);
		}
	}
}
