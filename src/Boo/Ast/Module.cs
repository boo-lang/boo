using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class Module : ModuleImpl
	{		
		public Module()
		{
			_using = new UsingCollection(this);
			Globals = new Block();
 		}
		
		public Module(Package package) : base(package)
		{
		}
		
		public Module(antlr.Token token, Package package) : base(token, package)
		{
		}
		
		internal Module(antlr.Token token) : base(token)
		{
		}
		
		internal Module(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override Package EnclosingPackage
		{
			get
			{
				return _package;
			}
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnModule(this);
		}
	}
}
