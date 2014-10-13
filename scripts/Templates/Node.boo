${header}
namespace Boo.Lang.Compiler.Ast

import System

public partial class ${node.Name}:

	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public def constructor():
		pass

	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public def constructor(lexicalInfo as LexicalInfo): 
		super(lexicalInfo)
