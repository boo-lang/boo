namespace BooExplorer.Common

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem
import System.IO

class CodeCompletionHunter(ProcessMethodBodies):
	
	static def GetCompletion(source as string):
		
		hunter = CodeCompletionHunter()
		compiler = BooCompiler()		
		compiler.Parameters.OutputWriter = StringWriter()
		compiler.Parameters.Pipeline = MakePipeline(hunter)
		compiler.Parameters.Input.Add(StringInput("none", source))
		compiler.Run()
	
		return array(IEntity, 0) unless hunter.Target
		return hunter.Target.GetMembers()

	[getter(Target)]
	_type as IType
	
	override protected def ProcessMemberReferenceExpression(node as MemberReferenceExpression):
		if node.Name == '__codecomplete__':
			_type = cast(IType, MyGetReferenceNamespace(node))

		super(node)
		
	protected def MyGetReferenceNamespace(expression as MemberReferenceExpression) as INamespace:
		target as Expression = expression.Target
		ns as INamespace = target.ExpressionType
		if ns is not null:
			return GetConcreteExpressionType(target)
		return cast(INamespace, GetEntity(target))
	
	protected static def MakePipeline(hunter):
		pipeline = Compile()
		index = pipeline.Find(Boo.Lang.Compiler.Steps.ProcessMethodBodies)
		pipeline[index] = hunter
		return pipeline
