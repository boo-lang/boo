namespace BooBinding

import System
import System.Collections
import System.CodeDom
import System.Text
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Ast.Visitors
import Boo.Lang.Parser

class CodeDomVisitor(DepthFirstVisitor):
	[Getter(codeCompileUnit)]
	_compileUnit = CodeCompileUnit()
	_namespace as CodeNamespace
	_class as CodeTypeDeclaration
	_statements as CodeStatementCollection
	
	def ConvModifiers(member as TypeMember) as MemberAttributes:
		return ConvModifiers(member.Modifiers)
	
	def ConvModifiers(modifier as TypeMemberModifiers) as MemberAttributes:
		attr as MemberAttributes = MemberAttributes.Private
		if (modifier & TypeMemberModifiers.Abstract) == TypeMemberModifiers.Abstract:
			attr = attr | MemberAttributes.Abstract
		if (modifier & TypeMemberModifiers.Final) == TypeMemberModifiers.Final:
			attr = attr | MemberAttributes.Const
		if (modifier & TypeMemberModifiers.Internal) == TypeMemberModifiers.Internal:
			attr = attr | MemberAttributes.Assembly
		if (modifier & TypeMemberModifiers.Override) == TypeMemberModifiers.Override:
			attr = attr | MemberAttributes.Override
		if (modifier & TypeMemberModifiers.Private) == TypeMemberModifiers.Private:
			attr = attr | MemberAttributes.Private
		if (modifier & TypeMemberModifiers.Protected) == TypeMemberModifiers.Protected:
			attr = attr | MemberAttributes.Family
		if (modifier & TypeMemberModifiers.Public) == TypeMemberModifiers.Public:
			attr = attr | MemberAttributes.Public
		if (modifier & TypeMemberModifiers.Static) == TypeMemberModifiers.Static:
			attr = attr | MemberAttributes.Static
		if (modifier & TypeMemberModifiers.Virtual) != TypeMemberModifiers.Virtual:
			attr = attr | MemberAttributes.Final
		return attr
	
	def ConvTypeRef(ref as TypeReference):
		return CodeTypeReference(ref.ToString())
	
	def OnCompileUnit(node as CompileUnit):
		_namespace = CodeNamespace("Global")
		_compileUnit.Namespaces.Add(_namespace)
		super(node)
	
	def OnNamespaceDeclaration(node as NamespaceDeclaration):
		_namespace = CodeNamespace(node.Name)
		_compileUnit.Namespaces.Add(_namespace)
	
	def OnImport(node as Import):
		_namespace.Imports.Add(CodeNamespaceImport(node.Namespace))
	
	def OnClassDefinition(node as ClassDefinition):
		oldClass = _class
		_class = CodeTypeDeclaration(node.Name)
		_class.IsClass = true
		
		for b as TypeReference in node.BaseTypes:
			_class.BaseTypes.Add(ConvTypeRef(b))
		
		super(node)
		
		_class = oldClass
	
	def OnField(node as Field):
		field = CodeMemberField(ConvTypeRef(node.Type), node.Name)
		field.Attributes = ConvModifiers(node)
		if node.Initializer != null:
			_expression = null
			Visit(node.Initializer)
			field.InitExpression = _expression
		_class.Members.Add(field)
	
	def OnMethod(node as Method):
		method = CodeMemberMethod()
		method.Name = node.Name
		method.Attributes = ConvModifiers(node)
		method.ReturnType = ConvTypeRef(node.ReturnType)
		for p as ParameterDeclaration in node.Parameters:
			method.Parameters.Add(CodeParameterDeclarationExpression(ConvTypeRef(p.Type), p.Name))
		_statements = method.Statements
		
		_class.Members.Add(method)
	
	def OnArrayLiteralExpression(node as ArrayLiteralExpression):
		pass
	
	def OnArrayTypeReference(node as ArrayTypeReference):
		pass
	
	def OnAsExpression(node as AsExpression):
		pass
	
	def OnAttribute(node as Boo.Lang.Compiler.Ast.Attribute):
		pass
	
	def OnBinaryExpression(node as BinaryExpression):
		pass
	
	def OnBlock(node as Block):
		pass
	
	def OnBoolLiteralExpression(node as BoolLiteralExpression):
		pass
	
	def OnBreakStatement(node as BreakStatement):
		pass
	
	def OnCallableBlockExpression(node as CallableBlockExpression):
		pass
	
	def OnCallableDefinition(node as CallableDefinition):
		pass
	
	def OnCallableTypeReference(node as CallableTypeReference):
		pass
	
	def OnCastExpression(node as CastExpression):
		pass
	
	def OnConstructor(node as Constructor):
		pass
	
	def OnContinueStatement(node as ContinueStatement):
		pass
	
	def OnDeclaration(node as Declaration):
		pass
	
	def OnDeclarationStatement(node as DeclarationStatement):
		pass
	
	def OnDoubleLiteralExpression(node as DoubleLiteralExpression):
		pass
	
	def OnEnumDefinition(node as EnumDefinition):
		pass
	
	def OnEvent(node as Event):
		pass
	
	def OnExceptionHandler(node as ExceptionHandler):
		pass
	
	def OnExpressionInterpolationExpression(node as ExpressionInterpolationExpression):
		pass
	
	def OnExpressionPair(node as ExpressionPair):
		pass
	
	def OnExpressionStatement(node as ExpressionStatement):
		pass
	
	def OnForStatement(node as ForStatement):
		pass
	
	def OnGeneratorExpression(node as GeneratorExpression):
		pass
	
	def OnGivenStatement(node as GivenStatement):
		pass
	
	def OnGotoStatement(node as GotoStatement):
		pass
	
	def OnHashLiteralExpression(node as HashLiteralExpression):
		pass
	
	def OnIfStatement(node as IfStatement):
		pass
	
	def OnIntegerLiteralExpression(node as IntegerLiteralExpression):
		pass
	
	def OnInterfaceDefinition(node as InterfaceDefinition):
		pass
	
	def OnLabelStatement(node as LabelStatement):
		pass
	
	def OnListLiteralExpression(node as ListLiteralExpression):
		pass
	
	def OnLocal(node as Local):
		pass
	
	def OnMacroStatement(node as MacroStatement):
		pass
	
	def OnMemberReferenceExpression(node as MemberReferenceExpression):
		pass
	
	def OnMethodInvocationExpression(node as MethodInvocationExpression):
		pass
	
	def OnModule(node as Module):
		pass
	
	def OnNullLiteralExpression(node as NullLiteralExpression):
		pass
	
	def OnOmittedExpression(node as OmittedExpression):
		pass
	
	def OnProperty(node as Property):
		pass
	
	def OnRaiseStatement(node as RaiseStatement):
		pass
	
	def OnReferenceExpression(node as ReferenceExpression):
		pass
	
	def OnRELiteralExpression(node as RELiteralExpression):
		pass
	
	def OnRetryStatement(node as RetryStatement):
		pass
	
	def OnReturnStatement(node as ReturnStatement):
		pass
	
	def OnSelfLiteralExpression(node as SelfLiteralExpression):
		pass
	
	def OnSimpleTypeReference(node as SimpleTypeReference):
		pass
	
	def OnSlice(node as Slice):
		pass
	
	def OnSlicingExpression(node as SlicingExpression):
		pass
	
	def OnStatementModifier(node as StatementModifier):
		pass
	
	def OnStringLiteralExpression(node as StringLiteralExpression):
		pass
	
	def OnStructDefinition(node as StructDefinition):
		pass
	
	def OnSuperLiteralExpression(node as SuperLiteralExpression):
		pass
	
	def OnTernaryExpression(node as TernaryExpression):
		pass
	
	def OnTimeSpanLiteralExpression(node as TimeSpanLiteralExpression):
		pass
	
	def OnTryStatement(node as TryStatement):
		pass
	
	def OnTypeofExpression(node as TypeofExpression):
		pass
	
	def OnUnaryExpression(node as UnaryExpression):
		pass
	
	def OnUnlessStatement(node as UnlessStatement):
		pass
	
	def OnUnpackStatement(node as UnpackStatement):
		pass
	
	def OnWhenClause(node as WhenClause):
		pass
	
	def OnWhileStatement(node as WhileStatement):
		pass
	
	def OnYieldStatement(node as YieldStatement):
		pass

