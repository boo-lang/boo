using Boo.Lang.Compiler.Ast;
using Boo.Lang.Parser.Util;
using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;

namespace Boo.Lang.ParserV4
{
	public delegate void ParserErrorHandler(IToken offendingSymbol, string filename, int line, int charPositionInLine, string msg, RecognitionException e);
	
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
	
		public static CompileUnit ParseReader(Boo.Lang.Parser.ParserSettings settings, string readerName, TextReader reader)
		{
			var cu = new CompileUnit();
			ParseModule(settings, cu, readerName, reader);
			return cu;
		}

		public static CompileUnit ParseReader(string readerName, TextReader reader, ParserErrorHandler eh)
		{
			var settings = new Boo.Lang.Parser.ParserSettings();
			settings.ErrorHandlerV4 = eh;
			return ParseReader(settings, readerName, reader);
		}
		
		public static CompileUnit ParseString(string name, string text)
		{
			return ParseReader(name, new StringReader(text), null);
		}
		
		public static Module ParseModule(Boo.Lang.Parser.ParserSettings settings, CompileUnit cu, string readerName, TextReader reader)
		{
			if (Readers.IsEmpty(reader))
			{
				var emptyModule = new Module(new LexicalInfo(readerName), Boo.Lang.Parser.CodeFactory.ModuleNameFrom(readerName));
				cu.Modules.Add(emptyModule);
				return emptyModule;
			}

			AntlrInputStream stream = new AntlrInputStream(reader);
			BooParser.StartContext tree;

			try
			{
				var parser = CreateParser(readerName, stream, true, settings.ErrorHandlerV4);
				tree = parser.start();
			}
			catch (ParseCanceledException)
			{
				stream.Seek(0);
				var parser = CreateParser(readerName, stream, false, settings.ErrorHandlerV4);
				tree = parser.start();
			}

			var visitor = new BooParserAstBuilderListener(cu, readerName);
			var module = visitor.VisitStart(tree);

			module.Name = Boo.Lang.Parser.CodeFactory.ModuleNameFrom(readerName);
			return module;
		}
		
		public static BooParser CreateParser(string readerName, ICharStream stream, bool firstStage, ParserErrorHandler eh)
		{
			ITokenSource lexer = new BooLexer(stream) { TokenFactory = BooTokenV4.TokenCreator };
			ITokenSource filter = new IndentTokenStreamFilterV4(lexer, BooLexer.WS, BooLexer.NEWLINE, BooLexer.INDENT, BooLexer.DEDENT, BooLexer.EOL, BooLexer.END, BooLexer.ID);
			ITokenStream tokens = new CommonTokenStream(filter);
			var parser = new BooParser(tokens);
			ParserATNSimulator interpreter = parser.Interpreter;
			interpreter.tail_call_preserves_sll = false;

			if (firstStage)
			{
				interpreter.PredictionMode = PredictionMode.Sll;
				parser.ErrorHandler = new BailErrorStrategy();
			}
			else
			{
				interpreter.enable_global_context_dfa = true;
				parser.ErrorHandler = new DefaultErrorStrategy();
			}

			parser.BuildParseTree = true;
			if (eh != null)
				parser.AddErrorListener(new BooErrorListener(eh, readerName));
			return parser;
		}
		
		public static CompileUnit ParseFile(string fname)
		{
			return ParseFile(new Boo.Lang.Parser.ParserSettings(), fname);
		}

		public static CompileUnit ParseFile(Boo.Lang.Parser.ParserSettings settings, string fname)
		{
			if (null == fname)
				throw new ArgumentNullException("fname");
	
			using (StreamReader reader = File.OpenText(fname))
				return ParseReader(settings, fname, reader);
		}
		
		public static Expression ParseExpression(string name, string text)
		{
			var parser = CreateParser(name, new AntlrInputStream(text), false, null);
			
			var expr = parser.expression();
			var visitor = new BooParserAstBuilderListener(new CompileUnit(), name);
			return visitor.VisitExpression(expr);
		}
		
	}
}
