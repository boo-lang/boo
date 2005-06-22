namespace Boo.Lang.Compiler.Steps
{
	using Boo.Lang.Compiler.Ast;

	public class RemoveDeadCode : AbstractTransformerCompilerStep
	{
		override public void Run()
		{
			Visit(CompileUnit);
		}

		override public void OnTryStatement(TryStatement node)
		{
			if (0 == node.ProtectedBlock.Statements.Count)
			{
				if (null != node.EnsureBlock && node.EnsureBlock.Statements.Count > 0)
				{
					ReplaceCurrentNode(node.EnsureBlock);
				}
				else
				{
					RemoveCurrentNode();
				}
			}
			else
			{
				base.OnTryStatement(node);
			}
		}
	}
}
