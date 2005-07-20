import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem

class EverybodyLovesDucks(ProcessMethodBodiesWithDuckTyping):
	
	_getInRuntime as IMethod
	
	override def OnReferenceExpression(node as ReferenceExpression):
		entity = self.NameResolutionService.Resolve(node.Name)
		if entity is not null:
			super(node)
		else:
			mie = CodeBuilder.CreateMethodInvocation(
					CodeBuilder.CreateSelfReference(self._currentMethod.DeclaringType),
					_getInRuntime)
			mie.Arguments.Add(CodeBuilder.CreateStringLiteral(node.Name))
			node.ParentNode.Replace(node, mie)
			
	override def InitializeMemberCache():
		super()
		_getInRuntime = TypeSystemServices.Map(typeof(BaseTemplate).GetMethod("GetInRuntime"))

class ProcessTemplate(AbstractVisitorCompilerStep):
	override def Run():
		Visit(self.CompileUnit)
		
	override def OnModule(node as Module):
		template = ClassDefinition(Name: "Template")
		template.BaseTypes.Add(CodeBuilder.CreateTypeReference(BaseTemplate))
		template.Members.Add(
			Method(Name: "Run", Body: node.Globals))
			
		node.Members.Add(template)
		node.Globals = Block()
		
class BaseTemplate:
	def GetInRuntime(name as string):
		return "name is: ${name}"
		
	abstract def Run():
		pass
		
template = """
print name
"""

pipeline = Pipelines.CompileToMemory()
pipeline.InsertAfter(InitializeTypeSystemServices, ProcessTemplate())
pipeline.Replace(ProcessMethodBodiesWithDuckTyping, EverybodyLovesDucks())

compiler = BooCompiler()
compiler.Parameters.Input.Add(IO.StringInput("template.boo", template))
compiler.Parameters.Pipeline = pipeline
compiler.Parameters.OutputType = CompilerOutputType.Library


try:
	result = compiler.Run()
	print result.Errors.ToString(true) if len(result.Errors)
	
	print "the generated code looks like this:"
	print
	print result.CompileUnit
	
	assembly = result.GeneratedAssembly	
	assert assembly is not null
	
	templateType = assembly.GetType("Template")
	templateInstance as BaseTemplate = templateType()
	templateInstance.Run()
	
except x:
	print x
