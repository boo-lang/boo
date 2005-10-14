${header}
namespace Boo.Lang.Compiler.Ast
{
	using System;
	
	[Serializable]	
	public class ${node.Name} : Boo.Lang.Compiler.Ast.Impl.${node.Name}Impl
	{
		public ${node.Name}()
		{
		}
		
		public ${node.Name}(Boo.Lang.Compiler.Ast.Node parent) : base(parent)
		{
		}
	}
}

