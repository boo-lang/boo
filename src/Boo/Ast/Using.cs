using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class Using : UsingImpl
	{		
		public Using()
		{
 		}
		
		public Using(string namespace_, ReferenceExpression assemblyReference, ReferenceExpression alias) : base(namespace_, assemblyReference, alias)
		{
		}
		
		public Using(antlr.Token token, string namespace_, ReferenceExpression assemblyReference, ReferenceExpression alias) : base(token, namespace_, assemblyReference, alias)
		{
		}
		
		internal Using(antlr.Token token) : base(token)
		{
		}
		
		internal Using(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnUsing(this);
		}
	}
}
