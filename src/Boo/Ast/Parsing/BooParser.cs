using System;
using System.IO;
using System.Text;
using Boo.Ast.Parsing.Util;

namespace Boo.Ast.Parsing
{
	public class BooParser : BooParserBase
	{	
		const int TabSize = 4;
		
		const string TokenObjectClass = "Boo.Ast.Parsing.BooToken";
		
		protected ParserErrorHandler Error;

		public BooParser(antlr.TokenStream lexer) : base(lexer)
		{
		}

		public static CompileUnit ParseFile(string fname)
		{
			if (null == fname)
			{
				throw new ArgumentNullException("fname");
			}
	
			using (StreamReader reader = File.OpenText(fname))
			{
				return ParseReader(fname, reader);
			}
		}

		public static CompileUnit ParseReader(string readerName, TextReader reader)
		{		
			CompileUnit cu = new CompileUnit();
			cu.Modules.Add(ParseModule(readerName, reader, null));
			return cu;
		}
	
		public static Module ParseModule(string readerName, TextReader reader, ParserErrorHandler errorHandler)
		{
			antlr.TokenStreamSelector selector = new antlr.TokenStreamSelector();
		
			BooLexer lexer = new BooLexer(reader);
			lexer.setTabSize(TabSize);
			lexer.setFilename(readerName);
			lexer.setTokenObjectClass(TokenObjectClass);
			lexer.Initialize(selector, TabSize, TokenObjectClass);
		
			IndentTokenStreamFilter filter = new IndentTokenStreamFilter(lexer, WS, INDENT, DEDENT, EOS);
			selector.select(filter);
		
			BooParser parser = new BooParser(selector);
			parser.setFilename(readerName);
			parser.Error += errorHandler;
		
			Module module = parser.start();
			module.Name = CreateModuleName(readerName);
			return module;
		}

		public override void reportError(antlr.RecognitionException x)
		{
			if (null != Error)
			{
				Error(x);
			}
			else
			{
				base.reportError(x);
			}
		}

		public static string CreateModuleName(string readerName)
		{
			if (readerName.IndexOfAny(Path.InvalidPathChars) > -1)
			{
				return EncodeModuleName(readerName);
			}
			return Path.GetFileNameWithoutExtension(Path.GetFileName(readerName));
		}

		static string EncodeModuleName(string name)
		{
			StringBuilder buffer = new StringBuilder(name.Length);
			foreach (char ch in name)
			{
				if (Char.IsLetterOrDigit(ch))
				{
					buffer.Append(ch);
				}
				else
				{
					buffer.Append("_");
				}
			}
			return buffer.ToString();
		}
	}
}
