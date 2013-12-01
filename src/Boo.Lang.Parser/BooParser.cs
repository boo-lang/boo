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
using System.Collections.Generic;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Environments;
using Boo.Lang.Parser.Util;
using antlr;

namespace Boo.Lang.Parser
{
	public class BooParser : BooParserBase
	{	
		protected ParserErrorHandler Error;

		/// <summary>
		/// Sets a limit to the depth the parser can go when recursing rules
		///</summary>
		/// <remarks>
		/// The parser must be build with the antlr `-traceParser` (Nant: antlr.trace=true) option 
		/// for this to have any actual effect.
		/// </remarks>		
		public uint MaxRecursionLimit { get; set; }
		protected Stack<string> RulesStack = new Stack<string>();

		public ErrorPattern[] ErrorPatterns { get; set; }

		protected int LastErrorLine = -1;

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
			return ParseFile(new ParserSettings(), fname);
		}

		[Obsolete]
		public static CompileUnit ParseFile(int tabSize, string fname)
		{
			var settings = new ParserSettings() { TabSize = tabSize };
			return ParseFile(settings, fname);
		}

		public static CompileUnit ParseFile(ParserSettings settings, string fname)
		{
			if (null == fname)
				throw new ArgumentNullException("fname");
	
			using (StreamReader reader = File.OpenText(fname))
				return ParseReader(settings, fname, reader);
		}
		
		public static CompileUnit ParseString(string name, string text)
		{
			return ParseReader(name, new StringReader(text));
		}
		
		public static CompileUnit ParseReader(string readerName, TextReader reader)
		{
			var settings = new ParserSettings();
			return ParseReader(settings, readerName, reader);
		}

		[Obsolete]
		public static CompileUnit ParseReader(int tabSize, string readerName, TextReader reader)
		{		
			var settings = new ParserSettings() { TabSize = tabSize };
			return ParseReader(settings, readerName, reader);
		}

		public static CompileUnit ParseReader(ParserSettings settings, string readerName, TextReader reader)
		{
			var cu = new CompileUnit();
			ParseModule(settings, cu, readerName, reader);
			return cu;
		}

		[Obsolete]
		public static Module ParseModule(int tabSize, CompileUnit cu, string readerName, TextReader reader, ParserErrorHandler errorHandler)
		{
			var settings = new ParserSettings() { TabSize = tabSize, ErrorHandler = errorHandler };
			return ParseModule(settings, cu, readerName, reader);
		}

		public static Module ParseModule(ParserSettings settings, CompileUnit cu, string readerName, TextReader reader)
		{
			if (Readers.IsEmpty(reader))
			{
				var emptyModule = new Module(new LexicalInfo(readerName), CodeFactory.ModuleNameFrom(readerName));
				cu.Modules.Add(emptyModule);
				return emptyModule;
			}

			var module = CreateParser(settings, readerName, reader).start(cu);
			module.Name = CodeFactory.ModuleNameFrom(readerName);
			return module;
		}

		[Obsolete]
		public static BooParser CreateParser(int tabSize, string readerName, TextReader reader, ParserErrorHandler errorHandler)
		{
			var settings = new ParserSettings() { TabSize = tabSize, ErrorHandler = errorHandler };
			return CreateParser(settings, readerName, reader);
		}

		public static BooParser CreateParser(ParserSettings settings, string readerName, TextReader reader)
		{
			var parser = new BooParser(CreateBooLexer(settings.TabSize, readerName, reader)) {
				MaxRecursionLimit = settings.MaxRecursionLimit,
				ErrorPatterns = settings.ErrorPatterns
			};
			parser.setFilename(readerName);
			if (null != settings.ErrorHandler)
				parser.Error += settings.ErrorHandler;
			return parser;
		}
		
		public static antlr.TokenStream CreateBooLexer(int tabSize, string readerName, TextReader reader)
		{
			var selector = new antlr.TokenStreamSelector();
		
			var lexer = new BooLexer(reader);
			lexer.setFilename(readerName);
			lexer.Initialize(selector, tabSize, BooToken.TokenCreator);
		
			var filter = new IndentTokenStreamFilter(lexer, WS, INDENT, DEDENT, EOL, END, ID);
			selector.select(filter);
			
			return selector;
		}

		override public void traceIn(string name)
		{
			if (MaxRecursionLimit > 0 && RulesStack.Count >= MaxRecursionLimit)
			{
				IToken token = LT(1);
				throw new RecognitionException(
					"Maximum recursion limit reached",
					token.getFilename(),
					token.getLine(),
					token.getColumn()
				);
			}
				
			RulesStack.Push(name);
		}

		override public void traceOut(string name)
		{
			RulesStack.Pop();
		}

		override public void reportError(RecognitionException x, string rulename)
		{
			// Silently ignore errors which are very close to a previous matched one,
			// assuming that they are produced while the parser is trying to recover.
			if (LastErrorLine != -1 && x.getLine() - LastErrorLine < 3) 
			{
				// Update it so we can extend the range as we go
				LastErrorLine = x.getLine();
				return;
			}

			// Override the reported error if there is matching pattern
			if (ErrorPatterns != null)
			{
				foreach (ErrorPattern pattern in ErrorPatterns)
				{
					if (pattern.Matches(rulename, x))
					{
						LastErrorLine = x.getLine();
						x = new RecognitionException(
							pattern.Message,
							x.getFilename(),
							x.getLine(),
							x.getColumn()
						);
						break;
					}
				}
			}

			reportError(x);
		}

		override public void reportError(RecognitionException x)
		{
			if (null != Error)
				Error(x);
			else
				base.reportError(x);
		}

		protected override void EmitIndexedPropertyDeprecationWarning(Property deprecated)
		{
			if (OutsideCompilationEnvironment())
				return;
			EmitWarning(
				CompilerWarningFactory.ObsoleteSyntax(
					deprecated,
					FormatPropertyWithDelimiters(deprecated, "(", ")"),
					FormatPropertyWithDelimiters(deprecated, "[", "]")));
		}

		protected override void EmitTransientKeywordDeprecationWarning(LexicalInfo location)
		{
			if (OutsideCompilationEnvironment())
				return;
			EmitWarning(
				CompilerWarningFactory.ObsoleteSyntax(
					location,
					"transient keyword",
					"[Transient] attribute"));
		}

		private void EmitWarning(CompilerWarning warning)
		{
			My<CompilerWarningCollection>.Instance.Add(warning);
		}

		private bool OutsideCompilationEnvironment()
		{
			return ActiveEnvironment.Instance == null;
		}

		private string FormatPropertyWithDelimiters(Property deprecated, string leftDelimiter, string rightDelimiter)
		{
			return deprecated.Name + leftDelimiter + Builtins.join(deprecated.Parameters, ", ") + rightDelimiter;
		}

		
	}
}
