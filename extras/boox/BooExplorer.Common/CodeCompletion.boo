namespace BooExplorer.Common

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem
import System.IO

class CodeCompletionHunter(ProcessMethodBodiesWithDuckTyping):
	
	static def GetCompletion(source as string):
		
		hunter = CodeCompletionHunter()
		compiler = BooCompiler()		
		compiler.Parameters.OutputWriter = StringWriter()
		compiler.Parameters.Pipeline = MakePipeline(hunter)
		compiler.Parameters.Input.Add(StringInput("none", source))
		result = compiler.Run()
		print(result.Errors.ToString(true))
		
		return hunter.Members

	[getter(Members)]
	_members = array(IEntity, 0)
	
	override protected def ProcessMemberReferenceExpression(node as MemberReferenceExpression):
		if node.Name == '__codecomplete__':
			_members = MyGetReferenceNamespace(node).GetMembers()
				
		super(node)
		
	protected def MyGetReferenceNamespace(expression as MemberReferenceExpression) as INamespace:		
		target as Expression = expression.Target
		
		print("ExpressionType: ${target.ExpressionType}")
		print("Entity: ${target.Entity}")
		
		if target.ExpressionType is not null:
			if target.ExpressionType.EntityType != EntityType.Error:
				return cast(INamespace, target.ExpressionType)
		return cast(INamespace, target.Entity)
	
	protected static def MakePipeline(hunter):
		pipeline = Compile()
		index = pipeline.Find(Boo.Lang.Compiler.Steps.ProcessMethodBodiesWithDuckTyping)
		pipeline[index] = hunter
		return pipeline
