using System;

namespace Boo.Ast
{
	public interface IAstTransformer
	{
		void OnCompileUnit(CompileUnit node, out CompileUnit newNode);
		void OnTypeReference(TypeReference node, out TypeReference newNode);
		void OnPackage(Package node, out Package newNode);
		void OnUsing(Using node, out Using newNode);
		void OnModule(Module node, out Module newNode);
		void OnClassDefinition(ClassDefinition node, out ClassDefinition newNode);
		void OnInterfaceDefinition(InterfaceDefinition node, out InterfaceDefinition newNode);
		void OnEnumDefinition(EnumDefinition node, out EnumDefinition newNode);
		void OnEnumMember(EnumMember node, out EnumMember newNode);
		void OnField(Field node, out Field newNode);
		void OnProperty(Property node, out Property newNode);
		void OnLocal(Local node, out Local newNode);
		void OnMethod(Method node, out Method newNode);
		void OnConstructor(Constructor node, out Constructor newNode);
		void OnParameterDeclaration(ParameterDeclaration node, out ParameterDeclaration newNode);
		void OnDeclaration(Declaration node, out Declaration newNode);
		void OnBlock(Block node, out Block newNode);
		void OnAttribute(Attribute node, out Attribute newNode);
		void OnStatementModifier(StatementModifier node, out StatementModifier newNode);
		void OnDeclarationStatement(DeclarationStatement node, out Statement newNode);
		void OnAssertStatement(AssertStatement node, out Statement newNode);
		void OnTryStatement(TryStatement node, out Statement newNode);
		void OnExceptionHandler(ExceptionHandler node, out ExceptionHandler newNode);
		void OnIfStatement(IfStatement node, out Statement newNode);
		void OnForStatement(ForStatement node, out Statement newNode);
		void OnWhileStatement(WhileStatement node, out Statement newNode);
		void OnGivenStatement(GivenStatement node, out Statement newNode);
		void OnWhenClause(WhenClause node, out WhenClause newNode);
		void OnBreakStatement(BreakStatement node, out Statement newNode);
		void OnContinueStatement(ContinueStatement node, out Statement newNode);
		void OnRetryStatement(RetryStatement node, out Statement newNode);
		void OnReturnStatement(ReturnStatement node, out Statement newNode);
		void OnYieldStatement(YieldStatement node, out Statement newNode);
		void OnRaiseStatement(RaiseStatement node, out Statement newNode);
		void OnUnpackStatement(UnpackStatement node, out Statement newNode);
		void OnExpressionStatement(ExpressionStatement node, out Statement newNode);
		void OnOmittedExpression(OmittedExpression node, out Expression newNode);
		void OnExpressionPair(ExpressionPair node, out ExpressionPair newNode);
		void OnMethodInvocationExpression(MethodInvocationExpression node, out Expression newNode);
		void OnUnaryExpression(UnaryExpression node, out Expression newNode);
		void OnBinaryExpression(BinaryExpression node, out Expression newNode);
		void OnTernaryExpression(TernaryExpression node, out Expression newNode);
		void OnReferenceExpression(ReferenceExpression node, out Expression newNode);
		void OnMemberReferenceExpression(MemberReferenceExpression node, out Expression newNode);
		void OnLiteralExpression(LiteralExpression node, out Expression newNode);
		void OnStringLiteralExpression(StringLiteralExpression node, out Expression newNode);
		void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node, out Expression newNode);
		void OnIntegerLiteralExpression(IntegerLiteralExpression node, out Expression newNode);
		void OnNullLiteralExpression(NullLiteralExpression node, out Expression newNode);
		void OnSelfLiteralExpression(SelfLiteralExpression node, out Expression newNode);
		void OnSuperLiteralExpression(SuperLiteralExpression node, out Expression newNode);
		void OnBoolLiteralExpression(BoolLiteralExpression node, out Expression newNode);
		void OnRELiteralExpression(RELiteralExpression node, out Expression newNode);
		void OnStringFormattingExpression(StringFormattingExpression node, out Expression newNode);
		void OnHashLiteralExpression(HashLiteralExpression node, out Expression newNode);
		void OnListLiteralExpression(ListLiteralExpression node, out Expression newNode);
		void OnTupleLiteralExpression(TupleLiteralExpression node, out Expression newNode);
		void OnListDisplayExpression(ListDisplayExpression node, out Expression newNode);
		void OnSlicingExpression(SlicingExpression node, out Expression newNode);
		void OnAsExpression(AsExpression node, out Expression newNode);
	}
}
