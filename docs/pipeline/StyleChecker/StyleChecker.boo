namespace StyleChecker

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.Pipelines

class StyleCheckerStep(AbstractVisitorCompilerStep):
	
	override def Run():
		Visit(CompileUnit)
		
	override def LeaveClassDefinition(node as ClassDefinition):
		if not System.Char.IsUpper(node.Name[0]):
			Error(node, "Class name '${node.Name}' does not start with uppercase letter!")
		
	override def LeaveField(node as Field):
		if not node.IsPublic:
			if not node.Name.StartsWith("_"):
				Error(node, "Field name '${node.Name}' does not start with '_'!")
			
	override def LeaveParameterDeclaration(node as ParameterDeclaration):
		if not System.Char.IsLower(node.Name[0]):
			Error(node, "Parameter name '${node.Name}' does not start with lowercase letter!")
			
	def Error(node as Node, message as string):
		Errors.Add(CompilerError(node, message))		

class StyleCheckerPipeline(CompileToFile):
	
	def constructor():
		self.Insert(1, StyleCheckerStep())
