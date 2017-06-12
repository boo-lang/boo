namespace Boo.Lang.CodeDom

import System
import System.CodeDom
import System.CodeDom.Compiler
import System.Collections.Generic
import System.IO
import Boo.Lang.Compiler
import Boo.Lang.Environments

class BooCodeParser(ICodeParser):
	
	private _references as (string)
	
	def constructor(references as (string)):
		_references = references
	
	public def Parse(codeStream as TextReader) as CodeCompileUnit:
		var text = codeStream.ReadToEnd()
		var input = Boo.Lang.Compiler.IO.StringInput('input', text)
		var compiler = BooCompiler()
		compiler.Parameters.Pipeline = Pipelines.ResolveExpressions()
		compiler.Parameters.Pipeline.AfterStep += {My[of CompilerContext].Instance.Errors.RemoveAll({e | e.Code.Equals('BCE0019')})}
		compiler.Parameters.Input.Add(input)
		AppDomain.CurrentDomain.GetAssemblies()
		var needed = HashSet[of string](_references)
		needed.Add('System.Windows.Forms')
		needed.Add('System.Drawing')
		var assemblies = AppDomain.CurrentDomain.GetAssemblies()
		for asm in assemblies:
			if asm.GetName().Name in needed:
				needed.Remove(asm.GetName().Name)
				compiler.Parameters.References.Add(asm)
		var context = compiler.Run()
		if context.Errors.Count > 0:
			raise context.Errors[0]
		var module = context.CompileUnit.Modules[0]
		var converter = BooCodeDomConverter()
		module.Accept(converter)
		var result = converter.CodeDomUnit
		result.UserData["text"] = text
		return result