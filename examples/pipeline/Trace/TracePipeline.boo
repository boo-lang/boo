namespace TracePipeline

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps

class TracePipelineStep(AbstractVisitorCompilerStep):
"""
	Visits every method adding a trace statement at both its very
	beginning and end.
"""	
	override def Run():
		Visit(CompileUnit)
		
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
		

class TracePipeline(CompileToFile):
	
	def constructor():
		self.Insert(1, TracePipelineStep())
