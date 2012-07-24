${header}
namespace Boo.Lang.Compiler.Ast
{
	[Serializable]	
	public partial class ${node.Name}
	{
		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public ${node.Name}()
		{
		}

		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public ${node.Name}(Node parent) : base(parent)
		{
		}
	}
}

