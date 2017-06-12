namespace Boo.Lang.ParserV4
{
    using Antlr4.Runtime;
    using System;

    public class BooErrorListener : BaseErrorListener
    {
        private ParserErrorHandler _errorHandler;
        private string _filename;

        public BooErrorListener(ParserErrorHandler eh, string filename)
        {
            this._errorHandler = eh;
            this._filename = filename;
        }

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            this._errorHandler(offendingSymbol, this._filename, line, charPositionInLine, msg, e);
        }
    }
}
