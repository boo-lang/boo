namespace TracePipeline

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Pipeline
import Boo.Lang.Compiler.Pipeline.Definitions

class TracePipelineStep(AbstractSwitcherCompilerStep):
	
	override def Run():
		Switch(CompileUnit)
		
	override def LeaveMethod(method as Method):
		stmt = TryStatement()
		stmt.ProtectedBlock = method.Body
		stmt.ProtectedBlock.Insert(0,
				CreatePrint("TRACE: Entering ${method.FullName}"))
		stmt.EnsureBlock = Block()
		stmt.EnsureBlock.Add(CreatePrint("TRACE: Leaving ${method.FullName}"))
		
		method.Body = Block()
		method.Body.Add(stmt)
		
	def CreatePrint(msg):
		// print(msg)
		mie = MethodInvocationExpression(ReferenceExpression("print"))
		mie.Arguments.Add(StringLiteralExpression(msg))
		return mie
		

class TracePipelineDefinition(BoocPipelineDefinition):
	
	override def Define(pipeline as CompilerPipeline):
		super(pipeline)
		pipeline.InsertAfter("parse", TracePipelineStep())
		// try to comment the previous line and uncomment
		// the following one to see the difference in output
		// pipeline.InsertAfter("normalization", TracePipelineStep())
