#region license
// Copyright (c) the Boo contributors
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace Boo.Lang.Parser;

using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Parser.Util;

public delegate void ParserErrorHandler(IRecognizer recognizer, IToken offendingSymbol, string filename, int line, int charPositionInLine, string msg, RecognitionException e);

partial class BooParser
{
	private static bool IsValidMacroArgument(int tokenType) =>
		LPAREN != tokenType && LBRACK != tokenType && DOT != tokenType && MULTIPLY != tokenType;

	// A name that runs straight into the end of the closure or the end of the
	// statement has no argument after it, so it is the value the closure
	// yields rather than a macro to invoke.
	protected bool IsValidClosureMacroArgument(int tokenType) =>
		IsValidMacroArgument(tokenType) && SUBTRACT != tokenType
		&& RBRACE != tokenType && EOS != tokenType;

	public static CompileUnit ParseReader(Boo.Lang.Parser.ParserSettings settings, string readerName, TextReader reader)
	{
		var cu = new CompileUnit();
		ParseModule(settings, cu, readerName, reader);
		return cu;
	}

	public static CompileUnit ParseReader(string readerName, TextReader reader, ParserErrorHandler eh)
	{
		var settings = new Boo.Lang.Parser.ParserSettings { ErrorHandler = eh };
		return ParseReader(settings, readerName, reader);
	}
	
	public static CompileUnit ParseString(string name, string text) =>
		ParseReader(name, new StringReader(text), null);
	
	public static Module ParseModule(Boo.Lang.Parser.ParserSettings settings, CompileUnit cu, string readerName, TextReader reader)
	{
		if (Readers.IsEmpty(reader))
		{
			var emptyModule = new Module(new LexicalInfo(readerName), Boo.Lang.Parser.CodeFactory.ModuleNameFrom(readerName));
			cu.Modules.Add(emptyModule);
			return emptyModule;
		}

		var stream = new AntlrInputStream(reader);
		BooParser.StartContext tree;

		try
		{
			var parser = CreateParser(settings.TabSize, readerName, stream, true, null);
			tree = parser.start();
		}
		catch (ParseCanceledException)
		{
			stream.Seek(0);
			var parser = CreateParser(settings.TabSize, readerName, stream, false, settings.ErrorHandler);
			tree = parser.start();
		}

		var visitor = new BooParserAstBuilderVisitor(cu, readerName);
		var module = visitor.VisitStart(tree);

		module.Name = Boo.Lang.Parser.CodeFactory.ModuleNameFrom(readerName);
		return module;
	}
	
	public static BooParser CreateParser(string readerName, ICharStream stream, bool firstStage, ParserErrorHandler eh) =>
		CreateParser(Boo.Lang.Parser.ParserSettings.DefaultTabSize, readerName, stream, firstStage, eh);

	public static BooParser CreateParser(int tabSize, string readerName, ICharStream stream, bool firstStage, ParserErrorHandler eh)
	{
		var booLexer = new BooLexer(stream) { TokenFactory = BooToken.CreateTokenFactory(tabSize) };
		// A lexer of its own reports to the console, so an unterminated string
		// would never reach the compiler.
		if (eh != null)
		{
			booLexer.RemoveErrorListeners();
			booLexer.AddErrorListener(new BooLexerErrorListener(eh, readerName));
		}
		var filter = new IndentTokenStreamFilter(booLexer, BooLexer.WS, BooLexer.NEWLINE, BooLexer.INDENT, BooLexer.DEDENT, BooLexer.EOL, BooLexer.END, BooLexer.ID);
		var parser = new BooParser(new CommonTokenStream(filter));

		// Two stage parsing. SLL is the faster prediction mode but rejects some
		// input the full LL mode accepts, so the first stage bails rather than
		// recovering and the caller retries from the top in LL.
		if (firstStage)
		{
			parser.Interpreter.PredictionMode = PredictionMode.SLL;
			parser.ErrorHandler = new BailErrorStrategy();
		}
		else
		{
			parser.Interpreter.PredictionMode = PredictionMode.LL;
			parser.ErrorHandler = new DefaultErrorStrategy();
		}

		parser.BuildParseTree = true;
		// Always drop ANTLR's console listener. A null handler means this stage
		// reports nothing, which is what the SLL pass wants.
		parser.RemoveErrorListeners();
		if (eh != null)
			parser.AddErrorListener(new BooErrorListener(eh, readerName));
		return parser;
	}
	
	public static CompileUnit ParseFile(string fname) =>
		ParseFile(new Boo.Lang.Parser.ParserSettings(), fname);

	public static CompileUnit ParseFile(Boo.Lang.Parser.ParserSettings settings, string fname)
	{
		if (null == fname)
			throw new ArgumentNullException(nameof(fname));

		using var reader = File.OpenText(fname);
		return ParseReader(settings, fname, reader);
	}
	
	public static Expression ParseExpression(string name, string text)
	{
		var parser = CreateParser(name, new AntlrInputStream(text), false, null);
		
		var expr = parser.expression();
		var visitor = new BooParserAstBuilderVisitor(new CompileUnit(), name);
		return visitor.VisitExpression(expr);
	}
	
}
