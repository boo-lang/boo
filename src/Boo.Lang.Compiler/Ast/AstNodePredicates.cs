using System.Linq;

namespace Boo.Lang.Compiler.Ast
{
	public static class AstNodePredicates
	{
		public static bool IsComplexSlicing(this SlicingExpression node)
		{
			return node.Indices.Any(IsComplexSlice);
		}

		public static bool IsTargetOfAssignment(this Expression node)
		{
			var parentExpression = node.ParentNode as BinaryExpression;
			if (parentExpression == null)
				return false;
			return node == parentExpression.Left && AstUtil.IsAssignment(parentExpression);
		}

		public static bool IsTargetOfMethodInvocation(this Expression node)
		{
			var parentExpression = node.ParentNode as MethodInvocationExpression;
			if (parentExpression == null)
				return false;
			return node == parentExpression.Target;
		}

	public static bool IsComplexSlice(Slice slice)
		{
			return slice.End != null
				|| slice.Step != null
				|| slice.Begin == OmittedExpression.Default;
		}
	}
}