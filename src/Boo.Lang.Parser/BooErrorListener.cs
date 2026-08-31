namespace Boo.Lang.Parser;

using Antlr4.Runtime;
using System;
using System.IO;

public class BooErrorListener : BaseErrorListener
{
	private ParserErrorHandler _errorHandler;
	private string _filename;

	public BooErrorListener(ParserErrorHandler eh, string filename)
	{
		this._errorHandler = eh;
		this._filename = filename;
	}

	public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
	{
		this._errorHandler(recognizer, offendingSymbol, this._filename, line, charPositionInLine, msg, e);
	}
}

/// <summary>
/// The lexer reports on character offsets rather than tokens, so it needs a
/// listener of its own. Without one it writes to the console and a bad token
/// never reaches the compiler.
/// </summary>
public class BooLexerErrorListener : IAntlrErrorListener<int>
{
	private ParserErrorHandler _errorHandler;
	private string _filename;

	public BooLexerErrorListener(ParserErrorHandler eh, string filename)
	{
		this._errorHandler = eh;
		this._filename = filename;
	}

	public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
	{
		this._errorHandler(recognizer, null, this._filename, line, charPositionInLine, msg, e);
	}
}
