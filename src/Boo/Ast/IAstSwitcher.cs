using System;

namespace Boo.Ast
{
	public interface IAstSwitcher : Boo.Util.ISwitcher
	{
		void OnCompileUnit(CompileUnit node);
		void OnTypeReference(TypeReference node);
		void OnPackage(Package node);
		void OnUsing(Using node);
		void OnModule(Module node);
		void OnClassDefinition(ClassDefinition node);
		void OnInterfaceDefinition(InterfaceDefinition node);
		void OnEnumDefinition(EnumDefinition node);
		void OnEnumMember(EnumMember node);
		void OnField(Field node);
		void OnProperty(Property node);
		void OnMethod(Method node);
		void OnConstructor(Constructor node);
		void OnParameterDeclaration(ParameterDeclaration node);
		void OnDeclaration(Declaration node);
		void OnBlock(Block node);
		void OnAttribute(Attribute node);
		void OnStatementModifier(StatementModifier node);
		void OnDeclarationStatement(DeclarationStatement node);
		void OnAssertStatement(AssertStatement node);
		void OnTryStatement(TryStatement node);
		void OnExceptionHandler(ExceptionHandler node);
		void OnIfStatement(IfStatement node);
		void OnForStatement(ForStatement node);
		void OnWhileStatement(WhileStatement node);
		void OnGivenStatement(GivenStatement node);
		void OnWhenClause(WhenClause node);
		void OnBreakStatement(BreakStatement node);
		void OnContinueStatement(ContinueStatement node);
		void OnRetryStatement(RetryStatement node);
		void OnReturnStatement(ReturnStatement node);
		void OnYieldStatement(YieldStatement node);
		void OnRaiseStatement(RaiseStatement node);
		void OnUnpackStatement(UnpackStatement node);
		void OnExpressionStatement(ExpressionStatement node);
		void OnOmittedExpression(OmittedExpression node);
		void OnExpressionPair(ExpressionPair node);
		void OnMethodInvocationExpression(MethodInvocationExpression node);
		void OnUnaryExpression(UnaryExpression node);
		void OnBinaryExpression(BinaryExpression node);
		void OnTernaryExpression(TernaryExpression node);
		void OnReferenceExpression(ReferenceExpression node);
		void OnMemberReferenceExpression(MemberReferenceExpression node);
		void OnLiteralExpression(LiteralExpression node);
		void OnStringLiteralExpression(StringLiteralExpression node);
		void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node);
		void OnIntegerLiteralExpression(IntegerLiteralExpression node);
		void OnNullLiteralExpression(NullLiteralExpression node);
		void OnSelfLiteralExpression(SelfLiteralExpression node);
		void OnSuperLiteralExpression(SuperLiteralExpression node);
		void OnBoolLiteralExpression(BoolLiteralExpression node);
		void OnRELiteralExpression(RELiteralExpression node);
		void OnStringFormattingExpression(StringFormattingExpression node);
		void OnHashLiteralExpression(HashLiteralExpression node);
		void OnListLiteralExpression(ListLiteralExpression node);
		void OnTupleLiteralExpression(TupleLiteralExpression node);
		void OnListDisplayExpression(ListDisplayExpression node);
		void OnSlicingExpression(SlicingExpression node);
		void OnAsExpression(AsExpression node);
	}
}
