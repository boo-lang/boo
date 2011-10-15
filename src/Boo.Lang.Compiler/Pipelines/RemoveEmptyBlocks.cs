using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;

namespace Boo.Lang.Compiler.Pipelines
{
	public class RemoveEmptyBlocks : AbstractTransformerCompilerStep
	{
		public override void LeaveBlock(Block node)
		{
			if (node.IsEmpty && node.ParentNode is Block)
				RemoveCurrentNode();
		}
	}
}