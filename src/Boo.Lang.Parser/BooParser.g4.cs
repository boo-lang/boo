namespace Boo.Lang.ParserV4
{
    partial class BooParser
    {
        private static bool IsValidMacroArgument(int tokenType)
        {
            return LPAREN != tokenType && LBRACK != tokenType && DOT != tokenType && MULTIPLY != tokenType;
        }

        protected bool IsValidClosureMacroArgument(int tokenType)
        {
            if (!IsValidMacroArgument(tokenType))
                return false;

            return SUBTRACT != tokenType;
        }
    }
}
