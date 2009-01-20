using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps.MacroProcessing
{
	class TypeMemberStatementBubbler : DepthFirstTransformer, ITypeMemberStatementVisitor
	{
		private TypeDefinition _current = null;

		protected override void OnNode(Node node)
		{
			TypeDefinition typeDefinition = node as TypeDefinition;
			if (null == typeDefinition)
			{
				base.OnNode(node);
				return;
			}

			TypeDefinition previous = _current;
			try
			{
				_current = typeDefinition;
				base.OnNode(node);
			}
			finally
			{
				_current = previous;
			}
		}

		#region Implementation of ITypeMemberStatementVisitor

		public void OnTypeMemberStatement(TypeMemberStatement node)
		{
			_current.Members.Add(node.TypeMember);
			Visit(node.TypeMember);
			RemoveCurrentNode();
		}

		#endregion
	}
}