namespace Boo.Microsoft.Build.Tasks

import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps

class PrepareScriptStep(AbstractCompilerStep):
	override def Run():
		module = CompileUnit.Modules[0]
		
		script = [|
			class __Script__(Boo.Microsoft.Build.Tasks.AbstractScript):
				override def Run():
					PrepareArgumentDictionary()
					$(module.Globals)
		|]
		
		script.Members.Extend(module.Members)

		module.Globals = Block()
				
		module.Members.Clear()
		module.Members.Add(script)