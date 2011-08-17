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
}
