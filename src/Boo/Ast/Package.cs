using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class Package : PackageImpl
	{		
		public Package()
		{
 		}
		
		public Package(string name) : base(name)
		{
		}
		
		public Package(antlr.Token token, string name) : base(token, name)
		{
		}
		
		internal Package(antlr.Token token) : base(token)
		{
		}
		
		internal Package(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnPackage(this);
		}
	}
}
