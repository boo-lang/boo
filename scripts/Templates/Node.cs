${header}
namespace Boo.Lang.Compiler.Ast
{
	using System;

	public partial class ${node.Name}
	{
		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public ${node.Name}()
		{
		}

		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public ${node.Name}(LexicalInfo lexicalInfo) : base(lexicalInfo)
		{
		}
	}
}

