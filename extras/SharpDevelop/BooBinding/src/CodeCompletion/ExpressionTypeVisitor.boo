namespace BooBinding.CodeCompletion

import System
import System.Collections
import SharpDevelop.Internal.Parser
import Boo.Lang.Compiler.Ast

class ExpressionTypeVisitor(DepthFirstVisitor):
	protected override def OnError(node as Node, error as Exception):
		BooParser.ShowException(error)
		super(node, error)
	
	[Property(ReturnType)]
	_returnType as IReturnType
	
	[Property(ReturnClass)]
	_returnClass as IClass
	
	[Property(Resolver)]
	_resolver as Resolver
	
	private def CreateReturnType(fullClassName as string):
		_returnClass = null
		if fullClassName == null:
			_returnType = null
		else:
			_returnType = BooBinding.CodeCompletion.ReturnType(fullClassName)
	
	private def CreateReturnType(reference as TypeReference):
		_returnClass = null
		if reference == null:
			_returnType = null
		else:
			_returnType = BooBinding.CodeCompletion.ReturnType(reference)
	
	private def CreateReturnType(c as IClass):
		_returnClass = c
		if c == null:
			_returnType = null
		else:
			_returnType = BooBinding.CodeCompletion.ReturnType(c)
	
	private def SetReturnType(r as IReturnType):
		_returnClass = null
		_returnType = r
	
	private def Debug(node):
		if node == null:
			print "-- null --"
		else:
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
		CreateReturnType("System.Delegate")
	
	override def OnExpressionStatement(node as ExpressionStatement):
		Debug(node)
		super(node)
	
	override def OnExpressionPair(node as ExpressionPair):
		Debug(node)
		super(node)
	
	override def OnMethodInvocationExpression(node as MethodInvocationExpression):
		Debug(node)
		Debug(node.Target)
		if node.Target isa MemberReferenceExpression:
			// call a method on another object
			mre as MemberReferenceExpression = node.Target
			Visit(mre.Target)
			if _returnClass == null and _returnType != null:
				_returnClass = _resolver.SearchType(_returnType.FullyQualifiedName)
			return if ProcessMethod(node, mre.Name, _returnClass)
		elif node.Target isa ReferenceExpression:
			re as ReferenceExpression = node.Target
			// try if it is a method on the current object
			return if ProcessMethod(node, re.Name, _resolver.CallingClass)
			// try if it is a class name -> constructor
			CreateReturnType(_resolver.SearchType(re.Name))
			return
		SetReturnType(null)
	
	private def ProcessMethod(node as MethodInvocationExpression, name as string, c as IClass) as bool:
		return false if c == null
		possibleOverloads = FindMethods(c, name, node.Arguments.Count)
		print "found ${possibleOverloads.Count} overloads (multiple overloads not supported yet)"
		if possibleOverloads.Count >= 1:
			SetReturnType(cast(IMethod, possibleOverloads[0]).ReturnType)
			return true
		/*// find best overload
		argumentTypes = array(IReturnType, node.Arguments.Count)
		for i as int in range(argumentTypes.Length):
			Visit(node.Arguments[i])
			argumentTypes[i] = _returnType
		...
		*/
		return false
	
	private def FindMethods(c as IClass, name as string, arguments as int):
		possibleOverloads = ArrayList()
		for cl as IClass in c.ClassInheritanceTree:
			for m as IMethod in cl.Methods:
				if m.Parameters.Count == arguments and name == m.Name:
					possibleOverloads.Add(m)
		return possibleOverloads
	
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
		// Resolve reference (to a variable, field, parameter or type)
		rt = _resolver.GetTypeFromLocal(node.Name)
		Debug(rt)
		if rt != null:
			SetReturnType(rt)
			return
		return if ProcessMember(node.Name, _resolver.CallingClass)
		CreateReturnType(_resolver.SearchType(node.Name))
	
	override def OnMemberReferenceExpression(node as MemberReferenceExpression):
		Debug(node)
		Visit(node.Target)
		if _returnClass == null and _returnType != null:
			_returnClass = _resolver.SearchType(_returnType.FullyQualifiedName)
		return if ProcessMember(node.Name, _returnClass)
		SetReturnType(null)
	
	private def ProcessMember(name as string, parentClass as IClass):
		return false if parentClass == null
		for cl as IClass in parentClass.ClassInheritanceTree:
			for c as IClass in cl.InnerClasses:
				if c.Name == name:
					CreateReturnType(c)
					return true
			for f as IField in cl.Fields:
				if f.Name == name:
					SetReturnType(f.ReturnType)
					return true
			for p as IProperty in cl.Properties:
				if p.Name == name:
					SetReturnType(p.ReturnType)
					return true
			for m as IMethod in cl.Methods:
				if m.Name == name:
					CreateReturnType("System.Delegate")
					return true
		return false
	
	override def OnTimeSpanLiteralExpression(node as TimeSpanLiteralExpression):
		CreateReturnType("System.TimeSpan")
	
	override def OnIntegerLiteralExpression(node as IntegerLiteralExpression):
		CreateReturnType("System.Int32")
	
	override def OnDoubleLiteralExpression(node as DoubleLiteralExpression):
		CreateReturnType("System.Double")
	
	override def OnNullLiteralExpression(node as NullLiteralExpression):
		CreateReturnType("System.Object")
	
	override def OnSelfLiteralExpression(node as SelfLiteralExpression):
		CreateReturnType(_resolver.CallingClass)
	
	override def OnSuperLiteralExpression(node as SuperLiteralExpression):
		CreateReturnType(_resolver.ParentClass)
	
	override def OnBoolLiteralExpression(node as BoolLiteralExpression):
		CreateReturnType("System.Boolean")
	
	override def OnRELiteralExpression(node as RELiteralExpression):
		CreateReturnType("System.Text.RegularExpressions.Regex")
	
	override def OnExpressionInterpolationExpression(node as ExpressionInterpolationExpression):
		Debug(node)
		super(node)
	
	override def OnHashLiteralExpression(node as HashLiteralExpression):
		CreateReturnType("System.Collections.Hashtable")
	
	override def OnListLiteralExpression(node as ListLiteralExpression):
		CreateReturnType("System.Collections.ArrayList")
	
	override def OnArrayLiteralExpression(node as ArrayLiteralExpression):
		CreateReturnType("System.Array")
	
	override def OnGeneratorExpression(node as GeneratorExpression):
		Debug(node)
		super(node)
	
	override def OnSlicingExpression(node as SlicingExpression):
		Debug(node)
		super(node)
	
	override def OnAsExpression(node as AsExpression):
		CreateReturnType(node.Type)
	
	override def OnCastExpression(node as CastExpression):
		CreateReturnType(node.Type)
	
	override def OnTypeofExpression(node as TypeofExpression):
		CreateReturnType("System.Type")
