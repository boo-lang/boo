${header}
namespace Boo.Lang.Compiler.Ast
{
	using System;
	
	[Serializable]	
	public partial class ${node.Name}
	{
		public ${node.Name}()
		{
		}
		
		public ${node.Name}(Boo.Lang.Compiler.Ast.Node parent) : base(parent)
		{
		}
	}
}

