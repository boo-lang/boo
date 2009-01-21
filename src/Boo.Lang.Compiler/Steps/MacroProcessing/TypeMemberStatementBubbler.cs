using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps.MacroProcessing
{
	class TypeMemberStatementBubbler : DepthFirstTransformer
	{
		#region Implementation of ITypeMemberStatementVisitor

		override public void OnCustomStatement(CustomStatement node)
		{
			TypeMemberStatement typeMemberStmt = node as TypeMemberStatement;
			if (null == typeMemberStmt)
				return;
			TypeMember typeMember = typeMemberStmt.TypeMember;
			node.GetAncestor<TypeDefinition>().Members.Add(typeMember);
			Visit(typeMember);
			RemoveCurrentNode();
		}

		#endregion
	}
}