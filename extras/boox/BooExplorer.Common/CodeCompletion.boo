namespace BooExplorer.Common

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem
import System.IO

class CompletionPointHunter(ProcessMethodBodies):

	[getter(Target)]
	_type as IType
	
	override protected def ProcessMemberReferenceExpression(node as MemberReferenceExpression):
		if node.Name == '__codecomplete__':
			_type = cast(IType, MyGetReferenceNamespace(node))

		super(node)
		
	def MyGetReferenceNamespace(expression as MemberReferenceExpression) as INamespace:
		target as Expression = expression.Target
		ns as INamespace = target.ExpressionType
		if ns is not null:
			return GetConcreteExpressionType(target)
		return cast(INamespace, GetEntity(target))

class CodeCompletion:
	_hunter = CompletionPointHunter()
	
	Members as (IEntity):
		get:
			return [].ToArray(IEntity) unless _hunter.Target
			return _hunter.Target.GetMembers()
			
	def constructor(source):
		compiler = BooCompiler()
		compiler.Parameters.References.Add(System.Reflection.Assembly.GetExecutingAssembly())
		compiler.Parameters.OutputWriter = StringWriter()
		compiler.Parameters.Pipeline = MakePipeline(CompileToMemory())
		compiler.Parameters.Input.Add(StringInput("none", source))

		result = compiler.Run()
	
	protected def MakePipeline(pipeline as CompilerPipeline):
		index = pipeline.Find(typeof(Boo.Lang.Compiler.Steps.ProcessMethodBodies))
		pipeline[index] = _hunter
		return pipeline
