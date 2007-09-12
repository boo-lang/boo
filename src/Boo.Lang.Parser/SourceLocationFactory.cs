using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Parser
{
	public class SourceLocationFactory
	{
		public static LexicalInfo ToLexicalInfo(antlr.IToken token)
		{
			return new LexicalInfo(token.getFilename(), token.getLine(), token.getColumn());
		}

		public static SourceLocation ToSourceLocation(antlr.IToken token)
		{
			return new SourceLocation(token.getLine(), token.getColumn());
		}

		public static SourceLocation ToEndSourceLocation(antlr.IToken token)
		{
			return new SourceLocation(token.getLine(), token.getColumn() + token.getText().Length - 1);
		}
	}
}
