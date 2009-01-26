using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem
{
	public class ImportAnnotations
	{
		private static object UsedAnnotation = new object();

		public static bool IsUsedImport(Import node)
		{
			return node.ContainsAnnotation(UsedAnnotation);
		}

		public static void MarkAsUsed(Import node)
		{
			if (IsUsedImport(node))
				return;
			node.Annotate(UsedAnnotation);
		}
	}
}
