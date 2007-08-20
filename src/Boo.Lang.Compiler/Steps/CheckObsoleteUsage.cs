namespace Boo.Lang.Compiler.Steps
{
	using System;
	using System.Reflection;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
    public class CheckObsoleteUsage: AbstractVisitorCompilerStep
	{
		override public void Run()
		{
			Visit(CompileUnit);
		}

		override public void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			OnReferenceExpression(node);
		}
		
		override public void OnReferenceExpression(ReferenceExpression node)
		{
			IExternalEntity member = TypeSystemServices.GetOptionalEntity(node) as IExternalEntity;
			if (member == null) return;
			
			System.Attribute[] attributes = System.Attribute.GetCustomAttributes(member.MemberInfo, typeof(ObsoleteAttribute));
			foreach (ObsoleteAttribute attr in attributes)
			{
				if (attr.IsError)
					Errors.Add(
						CompilerErrorFactory.Obsolete(node, member.ToString(), attr.Message));
				else
					Warnings.Add(
						CompilerWarningFactory.Obsolete(node, member.ToString(), attr.Message));
			}
		}
    }
}
