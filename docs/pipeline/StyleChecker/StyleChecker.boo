namespace StyleChecker

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.Pipelines

class StyleCheckerStep(AbstractVisitorCompilerStep):
	
	override def Run():
		Visit(CompileUnit)
		
	override def LeaveClassDefinition(clazz as ClassDefinition):
		if not System.Char.IsUpper(clazz.Name[0]):
			Error(clazz, "Class name '${clazz.Name}' does not start with uppercase letter!")
		
	override def LeaveField(field as Field):
		if not field.IsPublic:
			if not field.Name.StartsWith("_"):
				Error(field, "Field name '${field.Name}' does not start with '_'!")
			
	override def LeaveParameterDeclaration(param as ParameterDeclaration):
		if not System.Char.IsLower(param.Name[0]):
			Error(param, "Parameter name '${param.Name}' does not start with lowercase letter!")
			
	def Error(node as Node, message as string):
		Errors.Add(CompilerError(node, message))		

class StyleCheckerPipelineDefinition(CompileToFile):
	
	def constructor():
		self.Insert(1, StyleCheckerStep())
