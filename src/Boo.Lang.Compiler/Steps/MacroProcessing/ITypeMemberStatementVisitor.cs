using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps.MacroProcessing
{
	public interface ITypeMemberStatementVisitor : IAstVisitor
	{
		void OnTypeMemberStatement(TypeMemberStatement node);
	}
}