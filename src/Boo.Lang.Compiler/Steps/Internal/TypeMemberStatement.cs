using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps.Internal
{
	public class TypeMemberStatement : Statement
	{
		private TypeMember _member;

		public TypeMemberStatement(TypeMember member)
		{
			_member = member;
			_member.InitializeParent(this);
		}

		public TypeMember TypeMember
		{
			get { return _member;  }
		}

		#region Overrides of Node

		public override void Accept(IAstVisitor visitor)
		{
			ITypeMemberStatementVisitor typeMemberVisitor = visitor as ITypeMemberStatementVisitor;
			if (null != typeMemberVisitor)
				typeMemberVisitor.OnTypeMemberStatement(this);
			else
				_member.Accept(visitor);
		}

		public override NodeType NodeType
		{
			get { return Ast.NodeType.ExpressionStatement; }
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}

		#endregion
	}
}