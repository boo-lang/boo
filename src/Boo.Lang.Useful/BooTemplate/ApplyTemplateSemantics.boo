namespace Boo.Lang.Useful.BooTemplate

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem

class ApplyTemplateSemantics(AbstractCompilerStep):
	
	_compiler as TemplateCompiler
	
	def constructor(compiler as TemplateCompiler):
		_compiler = compiler
		
	override def Run():
		return if len(Errors) > 0
		
		assert 1 == len(CompileUnit.Modules)
		
		module = CompileUnit.Modules[0]
		template = ClassDefinition(Name: _compiler.TemplateClassName)
		template.BaseTypes.Add(CodeBuilder.CreateTypeReference(_compiler.TemplateBaseClass))
		template.Members.Extend(module.Members)
		
		execute = Method(Name: "Execute",
					ReturnType: CodeBuilder.CreateTypeReference(TypeSystemServices.VoidType),
					Modifiers: TypeMemberModifiers.Public|TypeMemberModifiers.Override)
		execute.Body = module.Globals
		template.Members.Add(execute)
		
		module.Members.Clear()
		module.Globals = Block()
		
		module.Members.Add(template)

