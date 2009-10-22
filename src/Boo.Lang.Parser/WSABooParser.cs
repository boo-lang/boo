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
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Parser.Util;

namespace Boo.Lang.Parser
{
	/// <summary>
	/// With this parser indentation is not used as
	/// a block delimiter but COLON end.
	///
	/// class Foo:
	/// def foo():
	///    print 'Hello'
	/// end
	/// end
	/// </summary>
	public class WSABooParser : WSABooParserBase
	{	
		protected Boo.Lang.Parser.ParserErrorHandler Error;

		public WSABooParser(antlr.TokenStream lexer) : base(lexer)
		{
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

		private static string ModuleNameFrom(string readerName)
		{
			return Boo.Lang.Parser.BooParser.ModuleNameFrom(readerName);
		}

		public static WSABooParser CreateParser(int tabSize, string readerName, TextReader reader, Boo.Lang.Parser.ParserErrorHandler errorHandler)
		{
			WSABooParser parser = new WSABooParser(CreateBooLexer(tabSize, readerName, reader));
			parser.setFilename(readerName);
			parser.Error += errorHandler;
			return parser;
		}
		
		public static antlr.TokenStream CreateBooLexer(int tabSize, string readerName, TextReader reader)
		{
			antlr.TokenStreamSelector selector = new antlr.TokenStreamSelector();		
			WSABooLexer lexer = new WSABooLexer(reader);
			lexer.setFilename(readerName);
			lexer.Initialize(selector, tabSize, Boo.Lang.Parser.BooToken.Creator);		
			selector.select(lexer);
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
	}
}
