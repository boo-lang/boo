namespace Boo.Lang.ParserV4
{
    using System;
    using System.Text;
    using Antlr4.Runtime;

    partial class BooLexer
    {
        protected int _skipWhitespaceRegion = 0;
        bool _preserveComments;

        [Obsolete]
        private StringBuilder text = new StringBuilder();

        [Obsolete]
        private int _begin
        {
            get
            {
                return _tokenStartCharIndex;
            }
        }

        private bool SkipWhitespace
        {
            get
            {
                return _skipWhitespaceRegion > 0;
            }
        }

        private static bool IsDigit(int ch)
        {
            return ch >= '0' && ch <= '9';
        }

        [Obsolete]
        private string getText()
        {
            return Text;
        }

        [Obsolete]
        private void setText(string text)
        {
            Text = text;
        }

        [Obsolete]
        private void setType(int type)
        {
            Type = type;
        }

        private void EnterSkipWhitespaceRegion()
        {
            _skipWhitespaceRegion++;
        }

        private void LeaveSkipWhitespaceRegion()
        {
            _skipWhitespaceRegion--;
        }

        private void ParseInterpolatedExpression(int closeTokenType, int openTokenType)
        {
            throw new NotImplementedException();
        }

        private void EnqueueInterpolatedToken(IToken token)
        {
            throw new NotImplementedException();
        }
    }
}
