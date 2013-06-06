using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Ast
{
	public static class NodeExtensions
	{
		public static T WithEntity<T>(this T node, IEntity entity) where T: Node
		{
			node.Entity = entity;
			return node;
		}
	}

	public static class MethodExtensions
	{
		public static bool IsPropertyAccessor(this Method method)
		{
			return null != method.ParentNode && method.ParentNode.NodeType == NodeType.Property;
		}
	}
}
