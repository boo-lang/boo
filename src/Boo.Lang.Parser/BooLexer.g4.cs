namespace Boo.Lang.ParserV4
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Antlr4.Runtime;

    partial class BooLexer
    {
        protected int _skipWhitespaceRegion = 0;

        private readonly Stack<int> _beginInterpolationType = new Stack<int>();
        private readonly Stack<int> _endInterpolationType = new Stack<int>();

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

        private void HandleInterpolatedExpression(int beginInterpolationType, int endTokenType)
        {
            _beginInterpolationType.Push(beginInterpolationType);
            _endInterpolationType.Push(endTokenType);
            PushMode(DefaultMode);
        }

        private void HandleInterpolationToken(int type)
        {
            if (_beginInterpolationType.Count == 0)
                return;

            if (_beginInterpolationType.Peek() == type)
            {
                PushMode(DefaultMode);
            }
            else if (_endInterpolationType.Peek() == type)
            {
                PopMode();
                if (_mode != DefaultMode)
                {
                    _beginInterpolationType.Pop();
                    _endInterpolationType.Pop();
                }
            }
        }
    }
}
