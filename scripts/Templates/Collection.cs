${header}
namespace Boo.Lang.Compiler.Ast
{
	using System;

	[Serializable]	
	public partial class ${node.Name}
	{
		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public ${node.Name}()
		{
		}

		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public ${node.Name}(Boo.Lang.Compiler.Ast.Node parent) : base(parent)
		{
		}
	}
}

