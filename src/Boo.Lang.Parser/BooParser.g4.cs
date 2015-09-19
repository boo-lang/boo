using Boo.Lang.Compiler.Ast;
using Boo.Lang.Parser.Util;
using System;
using System.IO;
using Antlr4.Runtime;

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
	
		public static CompileUnit ParseReader(Boo.Lang.Parser.ParserSettings settings, string readerName, TextReader reader)
		{
			var cu = new CompileUnit();
			ParseModule(settings, cu, readerName, reader);
			return cu;
		}

		public static CompileUnit ParseReader(string readerName, TextReader reader)
		{
			var settings = new Boo.Lang.Parser.ParserSettings();
			return ParseReader(settings, readerName, reader);
		}
		
		public static CompileUnit ParseString(string name, string text)
		{
			return ParseReader(name, new StringReader(text));
		}
		
		public static Module ParseModule(Boo.Lang.Parser.ParserSettings settings, CompileUnit cu, string readerName, TextReader reader)
		{
			if (Readers.IsEmpty(reader))
			{
				var emptyModule = new Module(new LexicalInfo(readerName), Boo.Lang.Parser.CodeFactory.ModuleNameFrom(readerName));
				cu.Modules.Add(emptyModule);
				return emptyModule;
			}

			var parser = CreateParser(readerName, reader);
			var tree = parser.start();
			var visitor = new BooParserAstBuilderListener(cu, readerName);
			var module = visitor.VisitStart(tree);

			module.Name = Boo.Lang.Parser.CodeFactory.ModuleNameFrom(readerName);
			return module;
		}
		
		public static BooParser CreateParser(string readerName, TextReader reader)
		{
			AntlrInputStream stream = new AntlrInputStream(reader);
			ITokenSource lexer = new BooLexer(stream) { TokenFactory = BooTokenV4.TokenCreator };
			ITokenSource filter = new IndentTokenStreamFilterV4(lexer, BooLexer.WS, BooLexer.NEWLINE, BooLexer.INDENT, BooLexer.DEDENT, BooLexer.EOL, BooLexer.END, BooLexer.ID);
			ITokenStream tokens = new CommonTokenStream(filter);
			var parser = new BooParser(tokens);
			parser.BuildParseTree = true;
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
			var parser = CreateParser(name, new StringReader(text));
			
			var expr = parser.expression();
			var visitor = new BooParserAstBuilderListener(new CompileUnit(), name);
			return visitor.VisitExpression(expr);
		}
		
	}
}
