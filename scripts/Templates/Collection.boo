${header}
namespace Boo.Lang.Compiler.Ast

[Serializable]	
public partial class ${node.Name}:

	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public def constructor():
		pass

	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public def constructor(parent as Node): 
		super(parent)
