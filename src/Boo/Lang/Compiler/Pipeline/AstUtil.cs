namespace Boo.Lang.Compiler.Pipeline
{
	using Boo.Lang.Ast;
	
	public class AstUtil
	{
		public static bool IsLhsOfAssignment(Expression node)
		{
			if (NodeType.BinaryExpression == node.ParentNode.NodeType)
			{
				BinaryExpression be = (BinaryExpression)node.ParentNode;
				if (BinaryOperatorType.Assign == be.Operator &&
					node == be.Left)
				{
					return true;
				}
			}
			return false;
		}
		
		private AstUtil()
		{
		}
	}
}
