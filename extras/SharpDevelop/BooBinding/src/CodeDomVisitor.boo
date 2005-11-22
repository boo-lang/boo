namespace BooBinding

import System
import System.Collections
import System.CodeDom
import System.Text
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Ast.Visitors
import Boo.Lang.Parser
import SharpDevelop.Internal.Parser

class CodeDomVisitor(IAstVisitor):
"""The CodeDomVisitor is able to convert from the Boo AST to System.CodeDom
It makes use of the SharpDevelop parser service to get necessary additional information about the
types."""
	[Getter(OutputCompileUnit)]
	_compileUnit = CodeCompileUnit()
	_namespace as CodeNamespace
	_class as CodeTypeDeclaration
	_statements as CodeStatementCollection
	_expression as CodeExpression
	
	def ConvModifiers(member as TypeMember) as MemberAttributes:
		if member isa Field:
			return ConvModifiers(member.Modifiers, MemberAttributes.Family)
		else:
			return ConvModifiers(member.Modifiers, MemberAttributes.Public)
	
	def ConvModifiers(modifier as TypeMemberModifiers, defaultAttr as MemberAttributes) as MemberAttributes:
		// Boo is not able to convert 0 to MemberAttributes, therefore we need to use
		// a trick to get the default value
		noAttr = MemberAttributes.Abstract & MemberAttributes.Final
		attr = noAttr
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
		if attr == noAttr:
			return defaultAttr
		else:
			return attr
	
	def ConvTypeRef(tr as TypeReference):
		return null if tr == null
		name = tr.ToString()
		expandedName = BooAmbience.ReverseTypeConversionTable[name]
		name = expandedName if expandedName != null
		return CodeTypeReference(name)
	
	def OnCompileUnit(node as CompileUnit):
		for m as Module in node.Modules:
			m.Accept(self)
	
	def OnModule(node as Module):
		if node.Namespace == null:
			_namespace = CodeNamespace("Global")
			_compileUnit.Namespaces.Add(_namespace)
		else:
			node.Namespace.Accept(self)
		for i as Import in node.Imports:
			i.Accept(self)
		for m as TypeMember in node.Members:
			m.Accept(self)
	
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
		
		for member as TypeMember in node.Members:
			member.Accept(self)
		
		if oldClass == null:
			_namespace.Types.Add(_class)
		else:
			oldClass.Members.Add(_class)
		_class = oldClass
	
	def OnStructDefinition(node as StructDefinition):
		oldClass = _class
		_class = CodeTypeDeclaration(node.Name)
		_class.IsStruct = true
		
		for b as TypeReference in node.BaseTypes:
			_class.BaseTypes.Add(ConvTypeRef(b))
		
		for member as TypeMember in node.Members:
			member.Accept(self)
		
		if oldClass == null:
			_namespace.Types.Add(_class)
		else:
			oldClass.Members.Add(_class)
		_class = oldClass
	
	def OnInterfaceDefinition(node as InterfaceDefinition):
		oldClass = _class
		_class = CodeTypeDeclaration(node.Name)
		_class.IsInterface = true
		
		for b as TypeReference in node.BaseTypes:
			_class.BaseTypes.Add(ConvTypeRef(b))
		
		for member as TypeMember in node.Members:
			member.Accept(self)
		
		if oldClass == null:
			_namespace.Types.Add(_class)
		else:
			oldClass.Members.Add(_class)
		_class = oldClass
	
	def OnField(node as Field):
		field = CodeMemberField(ConvTypeRef(node.Type), node.Name)
		field.Attributes = ConvModifiers(node)
		if node.Initializer != null:
			_expression = null
			//Visit(node.Initializer)
			field.InitExpression = _expression
		_class.Members.Add(field)
	
	def OnConstructor(node as Constructor):
		ConvertMethod(node, CodeConstructor())
	
	def OnMethod(node as Method):
		ConvertMethod(node, CodeMemberMethod(Name: node.Name))
		
	def OnDestructor(node as Destructor):
		ConvertMethod(node, CodeMemberMethod(Name: "Finalize"))
	
	def ConvertMethod(node as Method, method as CodeMemberMethod):
		method.Attributes = ConvModifiers(node)
		method.ReturnType = ConvTypeRef(node.ReturnType)
		if node.Parameters != null:
			for p as ParameterDeclaration in node.Parameters:
				method.Parameters.Add(CodeParameterDeclarationExpression(ConvTypeRef(p.Type), p.Name))
		_statements = method.Statements
		
		if node.Body != null:
			node.Body.Accept(self)
		
		_class.Members.Add(method)
	
	def OnArrayLiteralExpression(node as ArrayLiteralExpression):
		pass
	
	def OnArrayTypeReference(node as ArrayTypeReference):
		pass
	
	def OnTryCastExpression(node as TryCastExpression):
		pass
	
	def OnAttribute(node as Boo.Lang.Compiler.Ast.Attribute):
		pass
	
	def OnBinaryExpression(node as BinaryExpression):
		op = node.Operator
		if op == BinaryOperatorType.Assign:
			_expression = null
			node.Left.Accept(self)
			left = _expression
			_expression = null
			node.Right.Accept(self)
			if left != null and _expression != null:
				_statements.Add(CodeAssignStatement(left, _expression))
			_expression = null
			return
	
	def OnBlock(node as Block):
		for n as Statement in node.Statements:
			n.Accept(self)
	
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
	
	def OnContinueStatement(node as ContinueStatement):
		pass
	
	def OnDeclaration(node as Declaration):
		pass
	
	def OnDeclarationStatement(node as DeclarationStatement):
		pass
	
	def OnEnumDefinition(node as EnumDefinition):
		pass
	
	def OnEnumMember(node as EnumMember):
		pass
	
	def OnParameterDeclaration(node as ParameterDeclaration):
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
		_expression = null
		node.Expression.Accept(self)
		if _expression != null:
			_statements.Add(CodeExpressionStatement(_expression))
	
	def OnForStatement(node as ForStatement):
		pass
		
	def OnExtendedGeneratorExpression(node as ExtendedGeneratorExpression):
		pass
	
	def OnGeneratorExpression(node as GeneratorExpression):
		pass
	
	def OnGivenStatement(node as GivenStatement):
		pass
	
	def OnGotoStatement(node as GotoStatement):
		_statements.Add(CodeGotoStatement(node.Label.Name))
	
	def OnNullLiteralExpression(node as NullLiteralExpression):
		_expression = CodePrimitiveExpression(null)
	
	def OnBoolLiteralExpression(node as BoolLiteralExpression):
		_expression = CodePrimitiveExpression(node.Value)
	
	def OnStringLiteralExpression(node as StringLiteralExpression):
		_expression = CodePrimitiveExpression(node.Value)
	
	def OnCharLiteralExpression(node as CharLiteralExpression):
		_expression = CodePrimitiveExpression(node.Value)
	
	def OnHashLiteralExpression(node as HashLiteralExpression):
		pass
	
	def OnIntegerLiteralExpression(node as IntegerLiteralExpression):
		_expression = CodePrimitiveExpression(node.Value)
	
	def OnDoubleLiteralExpression(node as DoubleLiteralExpression):
		_expression = CodePrimitiveExpression(node.Value)
		
	def OnListLiteralExpression(node as ListLiteralExpression):
		pass
	
	def OnIfStatement(node as IfStatement):
		pass
	
	def OnLabelStatement(node as LabelStatement):
		pass
	
	def OnLocal(node as Local):
		pass
	
	def OnMacroStatement(node as MacroStatement):
		pass
	
	def OnMemberReferenceExpression(node as MemberReferenceExpression):
		_expression = null
		node.Target.Accept(self)
		if _expression != null:
			if _expression isa CodeTypeReferenceExpression:
				// TODO: lookup if expression is static member or subtype
				_expression = CodeTypeReferenceExpression("${cast(CodeTypeReferenceExpression, _expression).Type.BaseType}.${node.Name}")
			else:
				_expression = CreateMemberExpression(_expression, node.Name)
	
	def OnReferenceExpression(node as ReferenceExpression):
		p = GetParserService()
		if p.GetClass(node.Name) != null:
			_expression = CodeTypeReferenceExpression(node.Name)
		elif p.NamespaceExists(node.Name):
			_expression = CodeTypeReferenceExpression(node.Name)
		else:
			_expression = CreateMemberExpression(CodeThisReferenceExpression(), node.Name)
	
	def CreateMemberExpression(expr as CodeExpression, name as string):
		if expr isa CodeTypeReferenceExpression:
			typeRef = cast(CodeTypeReferenceExpression, _expression).Type.BaseType
			return CreateMemberExpression(expr, typeRef, name, true)
		elif expr isa CodeThisReferenceExpression:
			typeRef = "${_namespace.Name}.${_class.Name}"
			return CreateMemberExpression(expr, typeRef, name, false)
		return CodeFieldReferenceExpression(expr, name)
	
	def CreateMemberExpression(target as CodeExpression, parentName as string, name as string, isStatic as bool):
		combinedName = "${parentName}.${name}"
		p = GetParserService()
		parentClass = p.GetClass(parentName)
		if parentClass == null:
			if p.GetClass(combinedName) != null:
				return CodeTypeReferenceExpression(combinedName)
			elif p.NamespaceExists(combinedName):
				return CodeTypeReferenceExpression(combinedName)
		else:
			if isStatic:
				for innerClass as IClass in parentClass.InnerClasses:
					if innerClass.Name == name:
						return CodeTypeReferenceExpression(combinedName)
			for c as IClass in parentClass.ClassInheritanceTree:
				for ev as IEvent in c.Events:
					if ev.IsStatic == isStatic:
						return CodeEventReferenceExpression(target, name)
				for me as IMethod in c.Methods:
					if me.IsStatic == isStatic:
						return CodeMethodReferenceExpression(target, name)
				for prop as IProperty in c.Properties:
					if prop.IsStatic == isStatic:
						return CodePropertyReferenceExpression(target, name)
				for field as IField in c.Fields:
					if field.IsStatic == isStatic:
						return CodeFieldReferenceExpression(target, name)
		return CodeFieldReferenceExpression(target, name)
	
	def GetParserService() as ICSharpCode.SharpDevelop.Services.IParserService:
		return ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(ICSharpCode.SharpDevelop.Services.IParserService))
		
	def OnAstLiteralExpression(node as AstLiteralExpression):
		_expression = CodeObjectCreateExpression(node.Node.GetType())
	
	def OnMethodInvocationExpression(node as MethodInvocationExpression):
		_expression = null
		node.Target.Accept(self)
		if _expression != null:
			if _expression isa CodeTypeReferenceExpression:
				coce = CodeObjectCreateExpression(cast(CodeTypeReferenceExpression, _expression).Type)
				ConvertExpressions(coce.Parameters, node.Arguments)
				_expression = coce
			elif _expression isa CodeMethodReferenceExpression:
				cmie = CodeMethodInvokeExpression(_expression)
				ConvertExpressions(cmie.Parameters, node.Arguments)
				_expression = cmie
			elif _expression isa CodeFieldReferenceExpression:
				// when a type is unknown, a MemberReferenceExpression is translated into a CodeFieldReferenceExpression
				cfre as CodeFieldReferenceExpression = _expression
				cmie = CodeMethodInvokeExpression(cfre.TargetObject, cfre.FieldName)
				ConvertExpressions(cmie.Parameters, node.Arguments)
				_expression = cmie
			else:
				_expression = null
	
	def ConvertExpressions(args as CodeExpressionCollection, expressions as ExpressionCollection):
	"""Converts a list of expressions to CodeDom expressions."""
		for e in expressions:
			_expression = null
			e.Accept(self)
			args.Add(_expression)
	
	def OnOmittedExpression(node as OmittedExpression):
		pass
	
	def OnProperty(node as Property):
		pass
	
	def OnRaiseStatement(node as RaiseStatement):
		pass
	
	def OnRELiteralExpression(node as RELiteralExpression):
		pass
	
	def OnRetryStatement(node as RetryStatement):
		pass
	
	def OnReturnStatement(node as ReturnStatement):
		_expression = null
		if node.Expression != null:
			node.Expression.Accept(self)
		_statements.Add(CodeMethodReturnStatement(_expression))
	
	def OnSelfLiteralExpression(node as SelfLiteralExpression):
		_expression = CodeThisReferenceExpression()
	
	def OnSimpleTypeReference(node as SimpleTypeReference):
		pass
	
	def OnSlice(node as Slice):
		pass
	
	def OnSlicingExpression(node as SlicingExpression):
		pass
	
	def OnStatementModifier(node as StatementModifier):
		pass
	
	def OnSuperLiteralExpression(node as SuperLiteralExpression):
		_expression = CodeBaseReferenceExpression()
	
	def OnConditionalExpression(node as ConditionalExpression):
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
	
	def OnExplicitMemberInfo(node as ExplicitMemberInfo):
		pass
	
	def OnYieldStatement(node as YieldStatement):
		pass
		
	def OnGenericReferenceExpression(node as GenericReferenceExpression):
		pass
		
	def OnGenericTypeReference(node as GenericTypeReference):
		pass

