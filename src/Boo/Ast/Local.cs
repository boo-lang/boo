using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class Local : LocalImpl
	{		
		public Local(ReferenceExpression reference) : base(reference)
		{
			_name = reference.Name;
 		}
 		
 		public Local(Declaration declaration) : base(declaration)
 		{
 			_name = declaration.Name;
 		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnLocal(this);
		}
	}
}
