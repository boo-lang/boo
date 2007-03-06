namespace Boo.Lang.Useful.Attributes

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps

class NoTrace(AbstractAstAttribute):
	public static final Annotation = object()
	
	override def Apply(node as Node):
		node.Annotate(NoTrace.Annotation)

class TraceMethodCallsAttribute(AbstractAstAttribute):
"""
	Visits every method adding a trace statement at both its very
	beginning and end.
"""	

	[property(TraceMethod)]
	_traceMethod as ReferenceExpression
	
	def constructor():
		self(ast { System.Console.WriteLine });
		
	def constructor(traceMethod as ReferenceExpression):
		_traceMethod = traceMethod
		
	override def Apply(node as Node):
		node.Accept(TraceVisitor(_traceMethod))
		
	class IsGeneratorVisitor(DepthFirstVisitor):
		
		[getter(IsGenerator)]
		_isGenerator = false
		
		override def OnCallableBlockExpression(node as CallableBlockExpression):
			pass
		
		override def OnYieldStatement(node as YieldStatement):
			_isGenerator = true
		
	class TraceVisitor(DepthFirstVisitor):
		
		_traceMethod as ReferenceExpression
		
		def constructor(traceMethod as ReferenceExpression):
			_traceMethod = traceMethod
			
		def GetFullName(method as Method):
			if not IsModuleClass(method.DeclaringType):
				return method.FullName
			
			module as Module = method.EnclosingModule
			if module.Namespace is null: return method.Name
			return "${module.Namespace.Name}.{method.Name}"
			
		def IsModuleClass(type as TypeDefinition):
			return Boo.Lang.Compiler.Steps.IntroduceModuleClasses.IsModuleClass(type)
		
		override def LeaveMethod(method as Method):
			
			//print method, method.ContainsAnnotation(NoTrace.Annotation)
			if method.ContainsAnnotation(NoTrace.Annotation):
				return
			
			if not HasBody(method):
				return
			
			fullName = GetFullName(method)
			
			method.Body.Insert(0, TraceCall("TRACE: Entering ${fullName}"))
			if IsGenerator(method):
				return
				
			stmt = TryStatement()
			stmt.ProtectedBlock = method.Body
			stmt.EnsureBlock = Block()
			stmt.EnsureBlock.Add(TraceCall("TRACE: Leaving ${fullName}"))
			method.Body = Block()
			method.Body.Add(stmt)
			
		def IsGenerator(method as Method):
			visitor = IsGeneratorVisitor()
			method.Body.Accept(visitor)
			return visitor.IsGenerator
			
		def HasBody(method as Method):
			if method.DeclaringType isa InterfaceDefinition: return false
			if method.Body is null or len(method.Body.Statements) == 0: return false
			return true
			
		def TraceCall(msg as string):
			mie = MethodInvocationExpression(Target: _traceMethod.CloneNode())
			mie.Arguments.Add(StringLiteralExpression(msg))
			return mie
			
