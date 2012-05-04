using System.Linq;

namespace Boo.Lang.Compiler.Ast
{
	public static class AstNodePredicates
	{
		public static bool IsComplexSlicing(this SlicingExpression node)
		{
			return node.Indices.Any(AstUtil.IsComplexSlice);
		}

		public static bool IsTargetOfAssignment(this Expression node)
		{
			var parentExpression = node.ParentNode as BinaryExpression;
			if (parentExpression == null)
				return false;
			return node == parentExpression.Left && AstUtil.IsAssignment(parentExpression);
		}
	}
}