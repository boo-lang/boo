namespace Boo.Lang.CodeDom

import System
import System.CodeDom
import System.Linq.Enumerable
import Boo.Lang.Compiler.Ast

class BooCodeDomConverter(FastDepthFirstVisitor):

	[Getter(CodeDomUnit)]
	private _codeDomUnit as CodeCompileUnit

	private _defaultNS as CodeNamespace

	private _currentType as CodeTypeDeclaration

	private _currentMember as CodeTypeMember

	private _currentStatement as CodeExpressionStatement

	private _currentBlock as CodeStatementCollection

	public override def OnModule(node as Module):
		_codeDomUnit = CodeCompileUnit()
		_defaultNS = CodeNamespace();
		
		_codeDomUnit.Namespaces.Add(_defaultNS);
		super(node)

	private def ModifiersToDomAttributes(value as TypeMemberModifiers) as MemberAttributes:
		var result = Default(MemberAttributes)
		result |= MemberAttributes.Private if TypeMemberModifiers.Private & value
		result |= MemberAttributes.Family if TypeMemberModifiers.Internal & value
		result |= MemberAttributes.Family if TypeMemberModifiers.Protected & value
		result |= MemberAttributes.Public if TypeMemberModifiers.Public & value
		result |= MemberAttributes.Static if TypeMemberModifiers.Static & value
		result |= MemberAttributes.Final unless (TypeMemberModifiers.Virtual & value) or (TypeMemberModifiers.Override & value)
		result |= MemberAttributes.Override if TypeMemberModifiers.Override & value
		result |= MemberAttributes.Abstract if TypeMemberModifiers.Abstract & value
		result |= MemberAttributes.New if TypeMemberModifiers.New & value
		return result

	private def VisitAttributes(attrs as Boo.Lang.Compiler.Ast.Attribute*, coll as CodeAttributeDeclarationCollection):
		return if attrs is null
		for attr in attrs:
			var result = CodeAttributeDeclaration(attr.Name, attr.Arguments.Select({a | CodeAttributeArgument(VisitExpr(a))}))
			result.Arguments.AddRange(attr.NamedArguments.Select({na | CodeAttributeArgument(na.First.ToString(), VisitExpr(na.Second))}).ToArray())
			coll.Add(result)

	private def OnTypeMember(node as TypeMember, domNode as CodeTypeMember):
		domNode.Name = node.Name
		domNode.Attributes = ModifiersToDomAttributes(node.Modifiers)
		var oldMember = _currentMember
		VisitAttributes(node.Attributes, domNode.CustomAttributes)
		domNode.UserData['LexicalInfo'] = node.LexicalInfo
		_currentMember = oldMember

	private def OnTypeDefinition(node as TypeDefinition):
		var oldType = _currentType
		_currentType = CodeTypeDeclaration()
		OnTypeMember(node, _currentType)
		for baseType in node.BaseTypes:
			_currentType.BaseTypes.Add(baseType.ToString())
		Visit(node.GenericParameters)
		Visit(node.Members)
		if oldType is not null:
			oldType.Members.Add(_currentType)
		else: _defaultNS.Types.Add(_currentType)
		_currentType = oldType

	public override def OnClassDefinition(node as ClassDefinition):
		OnTypeDefinition(node)

	public override def OnCompileUnit(node as CompileUnit):
		raise NotImplementedException()

	public override def OnTypeMemberStatement(node as TypeMemberStatement):
		raise NotImplementedException()

	public override def OnExplicitMemberInfo(node as ExplicitMemberInfo):
		raise NotImplementedException()

	public override def OnSimpleTypeReference(node as SimpleTypeReference):
		raise NotImplementedException()

	public override def OnArrayTypeReference(node as ArrayTypeReference):
		raise NotImplementedException()

	public override def OnCallableTypeReference(node as CallableTypeReference):
		raise NotImplementedException()

	public override def OnGenericTypeReference(node as GenericTypeReference):
		raise NotImplementedException()

	public override def OnGenericTypeDefinitionReference(node as GenericTypeDefinitionReference):
		raise NotImplementedException()

	public override def OnCallableDefinition(node as CallableDefinition):
		raise NotImplementedException()

	public override def OnNamespaceDeclaration(node as NamespaceDeclaration):
		_defaultNS.Name = node.Name

	public override def OnImport(node as Import):
		pass

	public override def OnStructDefinition(node as StructDefinition):
		raise NotImplementedException()

	public override def OnInterfaceDefinition(node as InterfaceDefinition):
		raise NotImplementedException()

	public override def OnEnumDefinition(node as EnumDefinition):
		raise NotImplementedException()

	public override def OnEnumMember(node as EnumMember):
		raise NotImplementedException()

	private def VisitExpr(node as Expression) as CodeExpression:
		var oldStmt = _currentStatement
		_currentStatement = CodeExpressionStatement()
		try:
			Visit(node)
			return _currentStatement.Expression
		ensure:
			_currentStatement = oldStmt

	public override def OnField(node as Field):
		var fld = CodeMemberField()
		OnTypeMember(node, fld)
		if node.Type is not null:
			fld.Type = CodeTypeReference(fld.Type.ToString())
		fld.InitExpression = VisitExpr(node.Initializer)
		_currentType.Members.Add(fld)

	public override def OnProperty(node as Property):
		raise NotImplementedException()

	public override def OnEvent(node as Event):
		raise NotImplementedException()

	public override def OnLocal(node as Local):
		raise NotImplementedException()

	public override def OnBlockExpression(node as BlockExpression):
		raise NotImplementedException()

	private def VisitParameters(params as ParameterDeclaration*, collection as CodeParameterDeclarationExpressionCollection):
		for value in params:
			var newParam = CodeParameterDeclarationExpression(value.Type.ToString(), value.Name)
			newParam.Direction = FieldDirection.Ref if value.Modifiers == ParameterModifiers.Ref
			VisitAttributes(value.Attributes, newParam.CustomAttributes)
			collection.Add(newParam)

	private def VisitMethod(node as Method, result as CodeMemberMethod):
		result.Name = node.Name
		VisitParameters(node.Parameters, result.Parameters)
		#OnCallableDefinition(node, result) //this won't work. Implement it locally
		var oldBlock = _currentBlock
		_currentBlock = result.Statements
		Visit(node.Body)
		assert node.ImplementationFlags == MethodImplementationFlags.None //is this ever used anywhere?
		assert node.ExplicitInfo is null //TODO: implement this eventually
		_currentBlock = oldBlock
		_currentType.Members.Add(result)

	public override def OnMethod(node as Method):
		return if node.IsSynthetic
		VisitMethod(node, CodeMemberMethod())

	public override def OnConstructor(node as Constructor):
		return if node.IsSynthetic
		VisitMethod(node, CodeConstructor())

	public override def OnDestructor(node as Destructor):
		raise NotImplementedException()

	public override def OnParameterDeclaration(node as ParameterDeclaration):
		raise NotImplementedException()

	public override def OnGenericParameterDeclaration(node as GenericParameterDeclaration):
		raise NotImplementedException()

	public override def OnDeclaration(node as Declaration):
		raise NotImplementedException()

	public override def OnAttribute(node as Boo.Lang.Compiler.Ast.Attribute):
		raise NotImplementedException()

	public override def OnStatementModifier(node as StatementModifier):
		raise NotImplementedException()

	public override def OnGotoStatement(node as GotoStatement):
		raise NotImplementedException()

	public override def OnLabelStatement(node as LabelStatement):
		raise NotImplementedException()

	public override def OnDeclarationStatement(node as DeclarationStatement):
		raise NotImplementedException()

	public override def OnMacroStatement(node as MacroStatement):
		raise NotImplementedException()

	public override def OnTryStatement(node as TryStatement):
		raise NotImplementedException()

	public override def OnExceptionHandler(node as ExceptionHandler):
		raise NotImplementedException()

	public override def OnIfStatement(node as IfStatement):
		var result = CodeConditionStatement(VisitExpr(node.Condition))
		var oldBlock = _currentBlock
		if node.TrueBlock is not null:
			_currentBlock = result.TrueStatements
			Visit(node.TrueBlock)
		if node.FalseBlock is not null:
			_currentBlock = result.FalseStatements
			Visit(node.FalseBlock)
		_currentBlock = oldBlock
		_currentBlock.Add(result)

	public override def OnUnlessStatement(node as UnlessStatement):
		raise NotImplementedException()

	public override def OnForStatement(node as ForStatement):
		raise NotImplementedException()

	public override def OnWhileStatement(node as WhileStatement):
		raise NotImplementedException()

	public override def OnBreakStatement(node as BreakStatement):
		raise NotImplementedException()

	public override def OnContinueStatement(node as ContinueStatement):
		raise NotImplementedException()

	public override def OnReturnStatement(node as ReturnStatement):
		raise NotImplementedException()

	public override def OnYieldStatement(node as YieldStatement):
		raise NotImplementedException()

	public override def OnRaiseStatement(node as RaiseStatement):
		raise NotImplementedException()

	public override def OnUnpackStatement(node as UnpackStatement):
		raise NotImplementedException()

	private def ConvertAssign(be as BinaryExpression) as CodeAssignStatement:
		return CodeAssignStatement(VisitExpr(be.Left), VisitExpr(be.Right))

	public override def OnExpressionStatement(node as ExpressionStatement):
		var be = node.Expression as BinaryExpression
		if be is not null and be.Operator = BinaryOperatorType.Assign:
			_currentBlock.Add(ConvertAssign(be))
			return
		
		var result = CodeExpressionStatement()
		var oldSt = _currentStatement
		_currentStatement = result
		Visit(node.Expression)
		_currentBlock.Add(result)
		_currentStatement = oldSt

	public override def OnOmittedExpression(node as OmittedExpression):
		raise NotImplementedException()

	public override def OnExpressionPair(node as ExpressionPair):
		raise NotImplementedException()

	private def HandleConstructorInvocation(node as MethodInvocationExpression) as bool:
		return false unless node.Target.Entity.EntityType == Boo.Lang.Compiler.TypeSystem.EntityType.Constructor
		_currentStatement.Expression = CodeObjectCreateExpression(node.Target.ToCodeString(), *node.Arguments.Select(VisitExpr).ToArray())
		return true

	public override def OnMethodInvocationExpression(node as MethodInvocationExpression):
		return if HandleConstructorInvocation(node)
		var reference = CodeMethodReferenceExpression()
		if node.Target isa SuperLiteralExpression:
			reference.TargetObject = CodeBaseReferenceExpression()
		else:
			var re = node.Target cast ReferenceExpression
			reference.MethodName = re.Name
			var mre = node.Target as MemberReferenceExpression
			if mre is not null:
				if mre.Target isa MemberReferenceExpression:
					reference.TargetObject = CodeTypeReferenceExpression(mre.Target.ToCodeString())
				else: reference.TargetObject = VisitExpr(mre.Target)
			else:
				reference.TargetObject = CodeThisReferenceExpression()
		var result = CodeMethodInvokeExpression(reference, *node.Arguments.Select(VisitExpr).ToArray())
		_currentStatement.Expression = result

	public override def OnUnaryExpression(node as UnaryExpression):
		raise NotImplementedException()

	private static final OPS = {
		BinaryOperatorType.Addition : CodeBinaryOperatorType.Add,
		BinaryOperatorType.Subtraction : CodeBinaryOperatorType.Subtract,
		BinaryOperatorType.Multiply : CodeBinaryOperatorType.Multiply,
		BinaryOperatorType.Division : CodeBinaryOperatorType.Divide,
		BinaryOperatorType.Modulus : CodeBinaryOperatorType.Modulus,
		BinaryOperatorType.LessThan : CodeBinaryOperatorType.LessThan,
		BinaryOperatorType.LessThanOrEqual : CodeBinaryOperatorType.LessThanOrEqual,
		BinaryOperatorType.GreaterThan : CodeBinaryOperatorType.GreaterThan,
		BinaryOperatorType.GreaterThanOrEqual : CodeBinaryOperatorType.GreaterThanOrEqual,
		BinaryOperatorType.Equality : CodeBinaryOperatorType.ValueEquality,
		BinaryOperatorType.ReferenceEquality : CodeBinaryOperatorType.IdentityEquality,
		BinaryOperatorType.ReferenceInequality : CodeBinaryOperatorType.IdentityInequality,
		BinaryOperatorType.Or : CodeBinaryOperatorType.BooleanOr,
		BinaryOperatorType.And : CodeBinaryOperatorType.BitwiseAnd,
		BinaryOperatorType.BitwiseOr : CodeBinaryOperatorType.BitwiseOr,
		BinaryOperatorType.BitwiseAnd : CodeBinaryOperatorType.BitwiseAnd
		}

	private def ConvertBinaryOperator(op as BinaryOperatorType, ref result as CodeBinaryOperatorType) as bool:
		if OPS.ContainsKey(op):
			result = OPS[op]
			return true
		return false

	public override def OnBinaryExpression(node as BinaryExpression):
		op as CodeBinaryOperatorType
		if not ConvertBinaryOperator(node.Operator, op):
			_currentStatement.Expression = CodeSnippetExpression(node.ToCodeString())
			return
		var result = CodeBinaryOperatorExpression(VisitExpr(node.Left), op, VisitExpr(node.Right))
		_currentStatement.Expression = result

	public override def OnConditionalExpression(node as ConditionalExpression):
		raise NotImplementedException()

	public override def OnReferenceExpression(node as ReferenceExpression):
		_currentStatement.Expression = CodeArgumentReferenceExpression(node.Name)

	public override def OnMemberReferenceExpression(node as MemberReferenceExpression):
		if node.Target isa SelfLiteralExpression:
			_currentStatement.Expression = CodeFieldReferenceExpression(CodeThisReferenceExpression(), node.Name)
			return
		_currentStatement.Expression = CodePropertyReferenceExpression(VisitExpr(node.Target), node.Name)

	public override def OnGenericReferenceExpression(node as GenericReferenceExpression):
		raise NotImplementedException()

	public override def OnQuasiquoteExpression(node as QuasiquoteExpression):
		raise NotImplementedException()

	public override def OnStringLiteralExpression(node as StringLiteralExpression):
		_currentStatement.Expression = CodePrimitiveExpression(node.Value)

	public override def OnCharLiteralExpression(node as CharLiteralExpression):
		_currentStatement.Expression = CodePrimitiveExpression(node.Value cast string)

	public override def OnTimeSpanLiteralExpression(node as TimeSpanLiteralExpression):
		raise NotImplementedException()

	public override def OnIntegerLiteralExpression(node as IntegerLiteralExpression):
		_currentStatement.Expression = CodePrimitiveExpression(node.Value)

	public override def OnDoubleLiteralExpression(node as DoubleLiteralExpression):
		_currentStatement.Expression = CodePrimitiveExpression(node.Value)

	public override def OnNullLiteralExpression(node as NullLiteralExpression):
		_currentStatement.Expression = CodePrimitiveExpression(null)

	public override def OnSelfLiteralExpression(node as SelfLiteralExpression):
		_currentStatement.Expression = CodeThisReferenceExpression()

	public override def OnSuperLiteralExpression(node as SuperLiteralExpression):
		raise NotImplementedException()

	public override def OnBoolLiteralExpression(node as BoolLiteralExpression):
		_currentStatement.Expression = CodePrimitiveExpression(node.Value)

	public override def OnRELiteralExpression(node as RELiteralExpression):
		raise NotImplementedException()

	public override def OnSpliceExpression(node as SpliceExpression):
		raise NotImplementedException()

	public override def OnSpliceTypeReference(node as SpliceTypeReference):
		raise NotImplementedException()

	public override def OnSpliceMemberReferenceExpression(node as SpliceMemberReferenceExpression):
		raise NotImplementedException()

	public override def OnSpliceTypeMember(node as SpliceTypeMember):
		raise NotImplementedException()

	public override def OnSpliceTypeDefinitionBody(node as SpliceTypeDefinitionBody):
		raise NotImplementedException()

	public override def OnSpliceParameterDeclaration(node as SpliceParameterDeclaration):
		raise NotImplementedException()

	public override def OnExpressionInterpolationExpression(node as ExpressionInterpolationExpression):
		raise NotImplementedException()

	public override def OnHashLiteralExpression(node as HashLiteralExpression):
		raise NotImplementedException()

	public override def OnListLiteralExpression(node as ListLiteralExpression):
		raise NotImplementedException()

	public override def OnCollectionInitializationExpression(node as CollectionInitializationExpression):
		raise NotImplementedException()

	public override def OnArrayLiteralExpression(node as ArrayLiteralExpression):
		_currentStatement.Expression = CodeArrayCreateExpression(node.Type.ElementType.ToCodeString(), *node.Items.Select(VisitExpr).ToArray())

	public override def OnGeneratorExpression(node as GeneratorExpression):
		raise NotImplementedException()

	public override def OnExtendedGeneratorExpression(node as ExtendedGeneratorExpression):
		raise NotImplementedException()

	public override def OnSlice(node as Slice):
		raise NotImplementedException()

	public override def OnSlicingExpression(node as SlicingExpression):
		raise NotImplementedException()

	public override def OnTryCastExpression(node as TryCastExpression):
		raise NotImplementedException()

	public override def OnCastExpression(node as CastExpression):
		_currentStatement.Expression = CodeCastExpression(node.Type.ToCodeString(), VisitExpr(node.Target))

	public override def OnTypeofExpression(node as TypeofExpression):
		raise NotImplementedException()

	public override def OnCustomStatement(node as CustomStatement):
		raise NotImplementedException()

	public override def OnCustomExpression(node as CustomExpression):
		raise NotImplementedException()

	public override def OnFromClauseExpression(node as FromClauseExpression):
		raise NotImplementedException()

	public override def OnQueryContinuationExpression(node as QueryContinuationExpression):
		raise NotImplementedException()

	public override def OnSelectClauseExpression(node as SelectClauseExpression):
		raise NotImplementedException()

	public override def OnLetClauseExpression(node as LetClauseExpression):
		raise NotImplementedException()

	public override def OnWhereClauseExpression(node as WhereClauseExpression):
		raise NotImplementedException()

	public override def OnJoinClauseExpression(node as JoinClauseExpression):
		raise NotImplementedException()

	public override def OnGroupClauseExpression(node as GroupClauseExpression):
		raise NotImplementedException()

	public override def OnOrderByClauseExpression(node as OrderByClauseExpression):
		raise NotImplementedException()

	public override def OnOrderingExpression(node as OrderingExpression):
		raise NotImplementedException()

	public override def OnQueryExpression(node as QueryExpression):
		raise NotImplementedException()

	public override def OnStatementTypeMember(node as StatementTypeMember):
		raise NotImplementedException()
