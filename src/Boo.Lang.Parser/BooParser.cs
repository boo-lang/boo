#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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

using System;
using System.IO;
using System.Text;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Parser.Util;

namespace Boo.Lang.Parser
{
	public class BooParser : BooParserBase
	{	
		public const int DefaultTabSize = 4;
		
		protected ParserErrorHandler Error;

		public BooParser(antlr.TokenStream lexer) : base(lexer)
		{
		}
		
		public static Expression ParseExpression(int tabSize, string name, string text, ParserErrorHandler errorHandler)
		{
			return CreateParser(tabSize, name, new StringReader(text), errorHandler).expression();
		}
		
		public static Expression ParseExpression(string name, string text)
		{
			return ParseExpression(1, name, text, null);
		}
		
		public static CompileUnit ParseFile(string fname)
		{
			return ParseFile(DefaultTabSize, fname);
		}

		public static CompileUnit ParseFile(int tabSize, string fname)
		{
			if (null == fname)
			{
				throw new ArgumentNullException("fname");
			}
	
			using (StreamReader reader = File.OpenText(fname))
			{
				return ParseReader(tabSize, fname, reader);
			}
		}
		
		public static CompileUnit ParseString(string name, string text)
		{
			return ParseReader(name, new StringReader(text));
		}
		
		public static CompileUnit ParseReader(string readerName, TextReader reader)
		{
			return ParseReader(DefaultTabSize, readerName, reader);
		}

		public static CompileUnit ParseReader(int tabSize, string readerName, TextReader reader)
		{		
			CompileUnit cu = new CompileUnit();
			ParseModule(tabSize, cu, readerName, reader, null);
			return cu;
		}

		public static Module ParseModule(int tabSize, CompileUnit cu, string readerName, TextReader reader, ParserErrorHandler errorHandler)
		{
			if (Readers.IsEmpty(reader))
			{
				Module emptyModule = new Module(new LexicalInfo(readerName), ModuleNameFrom(readerName));
				cu.Modules.Add(emptyModule);
				return emptyModule;
			}

			Module module = CreateParser(tabSize, readerName, reader, errorHandler).start(cu);
			module.Name = ModuleNameFrom(readerName);
			return module;
		}

		public static BooParser CreateParser(int tabSize, string readerName, TextReader reader, ParserErrorHandler errorHandler)
		{
			BooParser parser = new BooParser(CreateBooLexer(tabSize, readerName, reader));
			parser.setFilename(readerName);
			parser.Error += errorHandler;
			return parser;
		}
		
		public static antlr.TokenStream CreateBooLexer(int tabSize, string readerName, TextReader reader)
		{
			antlr.TokenStreamSelector selector = new antlr.TokenStreamSelector();
		
			BooLexer lexer = new BooLexer(reader);
			lexer.setFilename(readerName);
			lexer.Initialize(selector, tabSize, BooToken.TokenCreator);
		
			IndentTokenStreamFilter filter = new IndentTokenStreamFilter(lexer, WS, INDENT, DEDENT, EOL);
			selector.select(filter);
			
			return selector;
		}

		override public void reportError(antlr.RecognitionException x)
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

		protected override void EmitIndexedPropertyDeprecationWarning(Property deprecated)
		{
			CompilerContext context = CompilerContext.Current;
			if (null == context)
				return;
			context.Warnings.Add(
				CompilerWarningFactory.ObsoleteSyntax(deprecated,
					FormatPropertyWithDelimiters(deprecated, "(", ")"),
					FormatPropertyWithDelimiters(deprecated, "[", "]")));
		}

		private string FormatPropertyWithDelimiters(Property deprecated, string leftDelimiter, string rightDelimiter)
		{
			return deprecated.Name + leftDelimiter + Builtins.join(deprecated.Parameters, ", ") + rightDelimiter;
		}

		override protected Module NewQuasiquoteModule(LexicalInfo li)
		{
			Module m = new Module(li);
			m.Name = ModuleNameFrom(li.FileName) + "$" + li.Line;
			return m;
		}
		
		public static string ModuleNameFrom(string readerName)
		{
			if (readerName.IndexOfAny(Path.GetInvalidPathChars()) > -1)
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
