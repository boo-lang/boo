using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem.Builders
{
	public interface ICodeBuilder
	{
		IntegerLiteralExpression CreateIntegerLiteral(int value);
	}
}