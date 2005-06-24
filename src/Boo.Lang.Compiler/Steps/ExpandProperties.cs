namespace Boo.Lang.Compiler.Steps
{
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;

	public class ExpandProperties : AbstractTransformerCompilerStep
	{
		override public void Run()
		{
			if (0 == Errors.Count)
			{
				Visit(CompileUnit);
			}
		}

		public override void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			if (null != node.Entity && EntityType.Property == node.Entity.EntityType)
			{
				if (!AstUtil.IsLhsOfAssignment(node))
				{
					ReplaceCurrentNode(
						CodeBuilder.CreatePropertyGet(
						node.Target,
						(IProperty)node.Entity));	
				}
			}
		}
	}
}
