using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps.Internal
{
	public interface ITypeMemberStatementVisitor : IAstVisitor
	{
		void OnTypeMemberStatement(TypeMemberStatement node);
	}
}