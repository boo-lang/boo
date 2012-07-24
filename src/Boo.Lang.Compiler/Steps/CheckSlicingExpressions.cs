using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps
{
	public class CheckSlicingExpressions : AbstractFastVisitorCompilerStep
	{
		public override void OnSlicingExpression(SlicingExpression node)
		{
			base.OnSlicingExpression(node);

			var arrayType = GetExpressionType(node.Target) as IArrayType;
			if (arrayType == null)
				return;

			if (arrayType.Rank != node.Indices.Count)
				Error(CompilerErrorFactory.InvalidArrayRank(node, node.Target.ToCodeString(), arrayType.Rank, node.Indices.Count));
		}

		public override void OnSlice(Slice node)
		{
			base.OnSlice(node);

			if (TypeSystemServices.IsDuckTyped((Expression) node.ParentNode))
				return;

			AssertInt(node.Begin);
			AssertOptionalInt(node.End);
			if (node.Step != null)
				CompilerErrorFactory.NotImplemented(node.Step, "slicing step");
		}

		private void AssertInt(Expression e)
		{
			AssertExpressionTypeIsCompatibleWith(TypeSystemServices.IntType, e);
		}

		private void AssertExpressionTypeIsCompatibleWith(IType expectedType, Expression e)
		{
			TypeChecker.AssertTypeCompatibility(e, expectedType, GetExpressionType(e));
		}

		private void AssertOptionalInt(Expression e)
		{
			if (IsNotOmitted(e))
				AssertInt(e);
		}

		private static bool IsNotOmitted(Expression e)
		{
			return e != null && e != OmittedExpression.Default;
		}

		private TypeChecker TypeChecker
		{
			get { return _typeChecker.Instance; }
		}

		public override void Dispose()
		{
			_typeChecker = new EnvironmentProvision<TypeChecker>();
			base.Dispose();
		}

		private EnvironmentProvision<TypeChecker> _typeChecker;
	}
}