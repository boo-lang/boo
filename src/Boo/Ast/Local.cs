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
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnLocal(this);
		}
	}
}
