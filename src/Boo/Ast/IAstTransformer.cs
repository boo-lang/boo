using System;

namespace Boo.Ast
{
	public interface IAstTransformer
	{
		void OnCompileUnit(CompileUnit node, ref CompileUnit newNode);
		void OnTypeReference(TypeReference node, ref TypeReference newNode);
		void OnPackage(Package node, ref Package newNode);
		void OnUsing(Using node, ref Using newNode);
		void OnModule(Module node, ref Module newNode);
		void OnClassDefinition(ClassDefinition node, ref ClassDefinition newNode);
		void OnInterfaceDefinition(InterfaceDefinition node, ref InterfaceDefinition newNode);
		void OnEnumDefinition(EnumDefinition node, ref EnumDefinition newNode);
		void OnEnumMember(EnumMember node, ref EnumMember newNode);
		void OnField(Field node, ref Field newNode);
		void OnProperty(Property node, ref Property newNode);
		void OnLocal(Local node, ref Local newNode);
		void OnMethod(Method node, ref Method newNode);
		void OnConstructor(Constructor node, ref Constructor newNode);
		void OnParameterDeclaration(ParameterDeclaration node, ref ParameterDeclaration newNode);
		void OnDeclaration(Declaration node, ref Declaration newNode);
		void OnBlock(Block node, ref Block newNode);
		void OnAttribute(Attribute node, ref Attribute newNode);
		void OnStatementModifier(StatementModifier node, ref StatementModifier newNode);
		void OnDeclarationStatement(DeclarationStatement node, ref Statement newNode);
		void OnAssertStatement(AssertStatement node, ref Statement newNode);
		void OnTryStatement(TryStatement node, ref Statement newNode);
		void OnExceptionHandler(ExceptionHandler node, ref ExceptionHandler newNode);
		void OnIfStatement(IfStatement node, ref Statement newNode);
		void OnForStatement(ForStatement node, ref Statement newNode);
		void OnWhileStatement(WhileStatement node, ref Statement newNode);
		void OnGivenStatement(GivenStatement node, ref Statement newNode);
		void OnWhenClause(WhenClause node, ref WhenClause newNode);
		void OnBreakStatement(BreakStatement node, ref Statement newNode);
		void OnContinueStatement(ContinueStatement node, ref Statement newNode);
		void OnRetryStatement(RetryStatement node, ref Statement newNode);
		void OnReturnStatement(ReturnStatement node, ref Statement newNode);
		void OnYieldStatement(YieldStatement node, ref Statement newNode);
		void OnRaiseStatement(RaiseStatement node, ref Statement newNode);
		void OnUnpackStatement(UnpackStatement node, ref Statement newNode);
		void OnExpressionStatement(ExpressionStatement node, ref Statement newNode);
		void OnOmittedExpression(OmittedExpression node, ref Expression newNode);
		void OnExpressionPair(ExpressionPair node, ref ExpressionPair newNode);
		void OnMethodInvocationExpression(MethodInvocationExpression node, ref Expression newNode);
		void OnUnaryExpression(UnaryExpression node, ref Expression newNode);
		void OnBinaryExpression(BinaryExpression node, ref Expression newNode);
		void OnTernaryExpression(TernaryExpression node, ref Expression newNode);
		void OnReferenceExpression(ReferenceExpression node, ref Expression newNode);
		void OnMemberReferenceExpression(MemberReferenceExpression node, ref Expression newNode);
		void OnLiteralExpression(LiteralExpression node, ref Expression newNode);
		void OnStringLiteralExpression(StringLiteralExpression node, ref Expression newNode);
		void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node, ref Expression newNode);
		void OnIntegerLiteralExpression(IntegerLiteralExpression node, ref Expression newNode);
		void OnNullLiteralExpression(NullLiteralExpression node, ref Expression newNode);
		void OnSelfLiteralExpression(SelfLiteralExpression node, ref Expression newNode);
		void OnSuperLiteralExpression(SuperLiteralExpression node, ref Expression newNode);
		void OnBoolLiteralExpression(BoolLiteralExpression node, ref Expression newNode);
		void OnRELiteralExpression(RELiteralExpression node, ref Expression newNode);
		void OnStringFormattingExpression(StringFormattingExpression node, ref Expression newNode);
		void OnHashLiteralExpression(HashLiteralExpression node, ref Expression newNode);
		void OnListLiteralExpression(ListLiteralExpression node, ref Expression newNode);
		void OnTupleLiteralExpression(TupleLiteralExpression node, ref Expression newNode);
		void OnListDisplayExpression(ListDisplayExpression node, ref Expression newNode);
		void OnSlicingExpression(SlicingExpression node, ref Expression newNode);
		void OnAsExpression(AsExpression node, ref Expression newNode);
	}
}
