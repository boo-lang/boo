namespace BooBinding.CodeCompletion

import System
import System.Collections
import SharpDevelop.Internal.Parser
import Boo.Lang.Compiler.Ast

class VariableLookupVisitor(DepthFirstVisitor):
	[Property(Resolver)]
	_resolver as Resolver
	
	[Property(LookFor)]
	_lookFor as string
	
	[Getter(ReturnType)]
	_returnType as IReturnType
	
	private def Finish(expr as Expression):
		return if expr == null
		return if _returnType != null
		visitor = ExpressionTypeVisitor(Resolver: _resolver)
		visitor.Visit(expr)
		_returnType = visitor.ReturnType
	
	private def Finish(reference as TypeReference):
		return if _returnType != null
		return if reference == null
		_returnType = BooBinding.CodeCompletion.ReturnType(reference)
	
	override def OnDeclaration(node as Declaration):
		return if node.Name != _lookFor
		Finish(node.Type)
	
	override def OnDeclarationStatement(node as DeclarationStatement):
		return if node.Declaration.Name != _lookFor
		Visit(node.Declaration)
		Finish(node.Initializer)
	
	override def OnBinaryExpression(node as BinaryExpression):
		if node.Operator == BinaryOperatorType.Assign and node.Left isa ReferenceExpression:
			reference as ReferenceExpression = node.Left
			if reference.Name == _lookFor:
				Finish(node.Right)
		super(node)

