namespace BooBinding.CodeCompletion

import System
import SharpDevelop.Internal.Parser
import Boo.Lang.Compiler.Ast

class ExpressionTypeVisitor(DepthFirstVisitor):
	protected override def OnError(node as Node, error as Exception):
		BooParser.ShowException(error)
		super(node, error)
	
	[Property(ReturnType)]
	returnType as IReturnType
	
	private def CreateReturnType(fullClassName as string):
		return BooBinding.CodeCompletion.ReturnType(fullClassName)
	
	private def CreateReturnType(reference as TypeReference):
		return BooBinding.CodeCompletion.ReturnType(reference)
	
	private def Debug(node as Node):
		print "${node.ToString()} - ${node.GetType().FullName}"
	
	override def OnSimpleTypeReference(node as SimpleTypeReference):
		Debug(node)
		super(node)
	
	override def OnArrayTypeReference(node as ArrayTypeReference):
		Debug(node)
		super(node)
	
	override def OnCallableTypeReference(node as CallableTypeReference):
		Debug(node)
		super(node)
	
	override def OnLocal(node as Local):
		Debug(node)
		super(node)
	
	override def OnCallableBlockExpression(node as CallableBlockExpression):
		Debug(node)
		returnType = CreateReturnType("System.Delegate")
	
	override def OnExpressionStatement(node as ExpressionStatement):
		Debug(node)
		super(node)
	
	override def OnExpressionPair(node as ExpressionPair):
		Debug(node)
		super(node)
	
	override def OnMethodInvocationExpression(node as MethodInvocationExpression):
		Debug(node)
		super(node)
	
	override def OnUnaryExpression(node as UnaryExpression):
		Debug(node)
		super(node)
	
	override def OnBinaryExpression(node as BinaryExpression):
		Debug(node)
		CombineTypes(node.Left, node.Right)
	
	override def OnTernaryExpression(node as TernaryExpression):
		Debug(node)
		CombineTypes(node.TrueValue, node.FalseValue)
	
	private def CombineTypes(a as Expression, b as Expression):
		Visit(a)
	
	override def OnReferenceExpression(node as ReferenceExpression):
		Debug(node)
		super(node)
	
	override def OnMemberReferenceExpression(node as MemberReferenceExpression):
		Debug(node)
		super(node)
	
	override def OnTimeSpanLiteralExpression(node as TimeSpanLiteralExpression):
		returnType = CreateReturnType("System.TimeSpan")
	
	override def OnIntegerLiteralExpression(node as IntegerLiteralExpression):
		returnType = CreateReturnType("System.Int32")
	
	override def OnDoubleLiteralExpression(node as DoubleLiteralExpression):
		returnType = CreateReturnType("System.Double")
	
	override def OnNullLiteralExpression(node as NullLiteralExpression):
		returnType = CreateReturnType("System.Object")
	
	override def OnSelfLiteralExpression(node as SelfLiteralExpression):
		//returnType = CreateReturnType(callingClass)
		pass
	
	override def OnSuperLiteralExpression(node as SuperLiteralExpression):
		//returnType = CreateReturnType(callingClass)
		pass
	
	override def OnBoolLiteralExpression(node as BoolLiteralExpression):
		returnType = CreateReturnType("System.Boolean")
	
	override def OnRELiteralExpression(node as RELiteralExpression):
		returnType = CreateReturnType("System.Text.RegularExpressions.Regex")
	
	override def OnExpressionInterpolationExpression(node as ExpressionInterpolationExpression):
		Debug(node)
		super(node)
	
	override def OnHashLiteralExpression(node as HashLiteralExpression):
		returnType = CreateReturnType("System.Collections.Hashtable")
	
	override def OnListLiteralExpression(node as ListLiteralExpression):
		returnType = CreateReturnType("System.Collections.ArrayList")
	
	override def OnArrayLiteralExpression(node as ArrayLiteralExpression):
		returnType = CreateReturnType("System.Array")
	
	override def OnGeneratorExpression(node as GeneratorExpression):
		Debug(node)
		super(node)
	
	override def OnSlicingExpression(node as SlicingExpression):
		Debug(node)
		super(node)
	
	override def OnAsExpression(node as AsExpression):
		returnType = CreateReturnType(node.Type)
	
	override def OnCastExpression(node as CastExpression):
		returnType = CreateReturnType(node.Type)
	
	override def OnTypeofExpression(node as TypeofExpression):
		returnType = CreateReturnType("System.Type")
